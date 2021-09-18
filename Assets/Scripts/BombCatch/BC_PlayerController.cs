using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BC_PlayerController : MonoBehaviour
{
    [SerializeField]
    float basketYSpacing = 0.15f;
    [SerializeField]
    int startingBasketCount = 3;
    [SerializeField]
    int basketCap = 5;
    [SerializeField]
    GameObject basketPrefab;

    [SerializeField]
    AudioSource collectedAudio;

    [SerializeField]
    Text _scoreText;
    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _scoreText.text = $"Score: {_score}";
            if (_score > HighScore)
            {
                HighScore = _score;
            }
        }
    }
    [SerializeField]
    Text highScoreText;
    private int _highScore;
    public int HighScore
    {
        get => _highScore;
        set
        {
            _highScore = value;
            highScoreText.text = $"High Score: {_highScore}";
            PlayerPrefs.SetInt("HighScore", _highScore);
            highScoreText.text = $"High Score: {_highScore}";
        }
    }

    // the array of baskets.
    // from 0 .. basketCount (exclusive) are all activated
    // from baskets[basketCount] onwards, all are deactivated
    private GameObject[] baskets;
    private int basketCount;

    private SpawnerBehavior spawner;
    private DinosaursController dinosaurs;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.Find("BombSpawner").GetComponent<SpawnerBehavior>();
        dinosaurs = GameObject.Find("Dinosaurs").GetComponent<DinosaursController>();

        // create our baskets
        baskets = new GameObject[basketCap];
        for (int i = 0; i < basketCap; ++i)
        {
            GameObject basket = Instantiate(basketPrefab, transform.position + i * basketYSpacing * Vector3.up, Quaternion.identity, transform);
            basket.SetActive(i < startingBasketCount);
            baskets[i] = basket;
        }
        basketCount = startingBasketCount;

        // initialize the high and current scores
        if (!PlayerPrefs.HasKey("HighScore"))
        {
            Debug.Log("initalizing HighScore key");
            PlayerPrefs.SetInt("HighScore", 0);
        }
        HighScore = PlayerPrefs.GetInt("HighScore");
        Score = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.x = Mathf.Clamp(pos.x, -5, 5);
        pos.y = transform.position.y;
        pos.z = transform.position.z;
        transform.position = pos;
    }

    public void GainBasket()
    {
        if (basketCount >= basketCap) return;
        baskets[basketCount].SetActive(true);
        ++basketCount;
    }
   
    public void GameOver()
    {
        Debug.Log("game over");
        spawner.StopSpawning();
        Invoke(nameof(_GameOver), 1f);
    }
    void _GameOver()
    {
        SceneManager.LoadScene("BombCatch_GameOver");
    }

    public void CollectedBomb()
    {
        ++Score;
        collectedAudio.Play();
        if (Score % 10 == 0 && Score > 0)
        {
            // every ten bombs, ramp up the difficulty
            spawner.IncreaseDifficulty();
        }
    }
    public void LoseLife(float delay)
    {
        dinosaurs.KillDinos();
        Invoke(nameof(_LoseLife), delay);
    }
    void _LoseLife()
    {
        --basketCount;
        baskets[basketCount].SetActive(false);
        if (basketCount <= 0)
        {
            GameOver();
            return;
        }
    }

    public float LowestYValue
    {
        get => transform.position.y;
    }

}
