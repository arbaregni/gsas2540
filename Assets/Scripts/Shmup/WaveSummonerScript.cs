using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveSummonerScript : MonoBehaviour
{
    [SerializeField]
    GameObject spawnIndicatorPrefab;

    [SerializeField]
    GameObject[] enemyPrefabs;

    [SerializeField]
    GameObject waveNotifier;

    [SerializeField]
    float countDownToWave;

    [SerializeField]
    int waveSize;

    [SerializeField]
    Bounds bounds;
    
    [SerializeField]
    float spawnRadius = 2f;


    TextCounter enemyCounter;
    Canvas canvas;

    public int WaveCount { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        enemyCounter = GameObject.Find("EnemyCounter").GetComponent<TextCounter>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        StartCoroutine(RepeatedWaves());
    }

    IEnumerator RepeatedWaves()
    {
        WaveCount = 0;
       
        while (true)
        {
            float endTime = Time.time + countDownToWave;
            float threshold = Time.time + 5f; // after 5 seconds we can summon the next wave early

            while (Time.time < endTime)
            {
                if (Time.time > threshold && enemyCounter.Count == 0)
                {
                    break;
                }
                yield return null;
            }

            Instantiate(waveNotifier, canvas.transform);

            yield return new WaitForSeconds(.2f);

            SummonWave();

            yield return new WaitForSeconds(.2f);

            waveSize += 1;
            if (waveSize > 30)
            {
                waveSize = 30;
            }
            
            if (WaveCount > 20)
            {
                countDownToWave /= 2;
            }
            WaveCount += 1;
        }
    }


    void SummonWave()
    {
        // create a random list of prefabs to instantiate
        List<GameObject> prefabs = new List<GameObject>(waveSize);
        for (int i = 0; i < waveSize; ++i)
        {
            int j = (int)(enemyPrefabs.Length * Random.value);
            prefabs.Add(enemyPrefabs[j]);
        }

        // find places to summon monsters
        List<Vector3> positions = new List<Vector3>(waveSize);
        for (int x = Mathf.FloorToInt(bounds.min.x); x <= bounds.max.x; ++x)
        {
            for (int y = Mathf.FloorToInt(bounds.min.y); y <= bounds.max.y; ++y)
            {

                Vector2 pos = new Vector2(x, y);
                Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, spawnRadius);
                if (colliders.Length == 0)
                {
                    positions.Add(pos);
                }
                else
                {
                    Debug.Log($"can't spawn at {pos}");
                }
            }
        }

        // now for each prefab we must instantiate, pick a random position to put it in
        for (int i = 0; i < waveSize; ++i)
        {
            if (positions.Count == 0)
            {
                Debug.Log($"not enough positions to summon all enemies, successfully summoned {i} / {waveSize} enemies");
                break;
            }
            // choose a random position to summon at
            int j = (int)(positions.Count * Random.value);

            // set the events in motion that will spawn the appropriate prefab
            SpawnIndicatorScript spawnIndicator = Instantiate(spawnIndicatorPrefab, positions[j], Quaternion.identity).GetComponent<SpawnIndicatorScript>();
            spawnIndicator.toSpawn = prefabs[i];

            // remove the position we just used
            positions[j] = positions[positions.Count - 1];
            positions.RemoveAt(positions.Count - 1);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
