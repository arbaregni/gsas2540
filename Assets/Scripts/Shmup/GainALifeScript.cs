using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainALifeScript : MonoBehaviour
{
    [SerializeField]
    float lifetime = 30f;

    [SerializeField]
    AudioSource pickupSound;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            IconTracker lifeCounter = GameObject.Find("LifeTracker").GetComponent<IconTracker>();
            if (lifeCounter.Count < lifeCounter.MaxCount)
            {
                lifeCounter.Count += 1;

                pickupSound.Play();
                pickupSound.transform.parent = transform.parent;

                Destroy(gameObject);
            }
        }
    }
}
