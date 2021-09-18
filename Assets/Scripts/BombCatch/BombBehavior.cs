using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : MonoBehaviour
{
    [SerializeField]
    AudioSource birthSound;


    // [SerializeField]
    static AudioSource explodeSound;

    static BC_PlayerController player = null;
    static SpawnerBehavior spawner;
    static List<BombBehavior> bombList = new List<BombBehavior>();

    // Start is called before the first frame update
    void Start()
    {
        birthSound.Play();

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<BC_PlayerController>();
            spawner = GameObject.Find("BombSpawner").GetComponent<SpawnerBehavior>();
            explodeSound = GameObject.Find("BoomAudio").GetComponent<AudioSource>();
        }
        bombList.Add(this);
        // rb.AddForce(0 * Vector2.up + 5 * Vector2.left, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Collector"))
        {
            bombList.Remove(this);
            player.CollectedBomb();
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            MissedBasket();
        }
    }

    void MissedBasket()
    {
        Debug.Log("player missed a bomb!");
        // stop all of the bombs from falling so we can blow them up
        spawner.StopSpawning();
        float delay = 0f;
        foreach (BombBehavior bomb in bombList)
        {
            delay = Mathf.Clamp(delay + 0.6f, 0.6f, 4f);
            bomb.StopAndBlowUp(delay);
        }
        delay += 0.6f;
        bombList.Clear();
        player.LoseLife(delay);
        spawner.ResumeSpawning(delay);
    }

    void StopAndBlowUp(float delay)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Invoke(nameof(BlowUp), delay);
    }
    void BlowUp()
    {
        explodeSound.Play();
        Destroy(this.gameObject);
    }

}
