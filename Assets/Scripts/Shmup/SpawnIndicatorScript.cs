using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIndicatorScript : MonoBehaviour
{
    public GameObject toSpawn;
    public float delay;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Spawn), delay);
    }

    void Spawn()
    {
        Instantiate(toSpawn, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
