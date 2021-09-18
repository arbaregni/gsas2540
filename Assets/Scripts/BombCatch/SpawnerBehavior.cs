using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    [SerializeField]
    GameObject bombPrefab;

    [SerializeField]
    float initialBombVelocity;

    [SerializeField]
    float movementInterval;

    [SerializeField]
    float speed;

    [SerializeField]
    float dropInterval;

    private Vector3 direction;
    private DinosaursController dinosaurs;
    private SpriteRenderer spriteRenderer;
    private bool doMovement;


    // Start is called before the first frame update
    void Start()
    {
        dinosaurs = GameObject.Find("Dinosaurs").GetComponent<DinosaursController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        doMovement = true;
        direction = Vector3.right;
        if (Random.value <= 0.5)
        {
            direction *= -1;
        }
        Invoke(nameof(ChangeMovement), movementInterval);
        Invoke(nameof(DropBomb), dropInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (doMovement)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
        if (transform.position.x < -5 || 5 < transform.position.x)
        {
            direction = Vector3.Reflect(direction, Vector3.right);
            spriteRenderer.flipX = direction.x > 0;
            float x = Mathf.Clamp(transform.position.x, -5, 5);
            transform.position = new Vector3(x, transform.position.y, 0f);
        }
    }


    void ChangeMovement()
    {
        doMovement = true;
        if (Random.value <= 0.33f)
        {
            direction *= -1;
            spriteRenderer.flipX = direction.x > 0;
        }
        Invoke(nameof(ChangeMovement), movementInterval);
    }

    void DropBomb()
    {
        if (Random.value <= 0.5f)
        {
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.GetComponent<Rigidbody2D>().AddForce(initialBombVelocity * Vector2.down, ForceMode2D.Impulse);
        }    
        Invoke(nameof(DropBomb), dropInterval);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(DropBomb));
        CancelInvoke(nameof(ChangeMovement));
        doMovement = false;
    }
    public void ResumeSpawning(float delay)
    {
        dinosaurs.ComeBackDinos();
        Invoke(nameof(DropBomb), delay);
        Invoke(nameof(ChangeMovement), delay);
    }


    public void IncreaseDifficulty()
    {
        // play an increase in difficulty animation
        Debug.Log("increasing difficulty");
        dropInterval = Mathf.Max(0.75f * dropInterval, 0.01f);
        speed += 2f;
    }



}
