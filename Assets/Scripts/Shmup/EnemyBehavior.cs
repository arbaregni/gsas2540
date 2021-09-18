using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBehavior : QuadMover, IStrikable
{
    [SerializeField]
    float fireInterval = 1f;

    [SerializeField]
    float sightDistance = 10f;

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    AudioSource prepareToFireSound;

    [SerializeField]
    AudioSource deathSound;

    [SerializeField]
    GameObject exclaim;

    [SerializeField]
    float dropProbability;

    [SerializeField]
    GameObject dropPrefab;

    private float lastFireTime = 0f;

    private static GameObject player;
    private static TextCounter scoreCounter;
    private static TextCounter enemyCounter;


    // Start is called before the first frame update
    public override void Start()
    {
        // choose a direction at random
        if (Random.value < 0.5f)
        {
            horizontality = (Random.value < 0.5f) ? 1 : -1;
            verticality = 0;
        }
        else
        {
            horizontality = 0;
            verticality = (Random.value < 0.5f) ? 1 : -1;
        }

        base.Start();

        if (player == null)
        {
            player = GameObject.Find("Player");
            scoreCounter = GameObject.Find("ScoreText").GetComponent<TextCounter>();
            enemyCounter = GameObject.Find("EnemyCounter").GetComponent<TextCounter>();
        }

        enemyCounter.Count += 1;
         // StartCoroutine(State_Idling());
        StartCoroutine(State_Wandering());
    }

    IEnumerator State_Wandering()
    {
        while (true)
        {
            // choose a direction at random
            if (Random.value < 0.5f)
            {
                horizontality = (Random.value < 0.5f) ? 1 : -1;
                verticality = 0;
            }
            else
            {
                horizontality = 0;
                verticality = (Random.value < 0.5f) ? 1 : -1;
            }

            // stop for a tiny bit after turning
            float delay = Mathf.Pow(Random.Range(0f, 1f), 2f);
            yield return new WaitForSeconds(delay);


            // 33% chance we'll stop and smell the flowers
            Speed = (Random.value < 0.33f) ? 0 : fullSpeed / 2f;

            // now walk (or wait) for a bit, and watch for the player as we do so
            float end = Time.time + Random.Range(1f, 3f);

            while (Time.time < end)
            {
                if (FindPlayer().HasValue)
                {
                    StartCoroutine(State_Pursuing());
                    yield break;
                }
                yield return null;
            }

            yield return null;
        }
        yield break;
    }

    IEnumerator State_Pursuing()
    {
        // first we stop and exclaim about finding the player
        Speed = 0f;
        exclaim.SetActive(true);
        yield return new WaitForSeconds(.5f);
        exclaim.SetActive(false);
        Speed = fullSpeed;

        while (true)
        {
            Vector3? target = FindPlayer();
            if (target.HasValue)
            {
                // look towards the target
                // we look up, down, left or right depending on which quarter of the plane the target lies in.
                //
                //    \   up  /
                //     \     /
                //      \   /
                //       \ /
                // left   X    right
                //       / \
                //      /   \
                //     /     \
                //    /  down \

                Vector3 diff = target.Value - transform.position;
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                {
                    horizontality = (int)Mathf.Sign(diff.x);
                    verticality = 0;
                }
                else
                {
                    horizontality = 0;
                    verticality = (int)Mathf.Sign(diff.y);
                }

                if (diff.magnitude > 3f)
                {
                    // far enough away that we want to chase them
                    Speed = fullSpeed;
                }
                else
                {
                    Speed = 0f;
                }

                
                if (Time.time - lastFireTime > fireInterval)
                {
                    // before we fire, play the hark sound
                    lastFireTime = Time.time;
                    prepareToFireSound.Play();
                    Invoke(nameof(FireProjectile), prepareToFireSound.clip.length + .1f);
                }
            }
            else
            {
                // lost the player, go back to wandering
                StartCoroutine(State_Wandering());
                break;
            }

            yield return null;
        }

        yield break;
    }

    void FireProjectile()
    {
        Instantiate(projectilePrefab, transform.position, HeadingAsQuaternion);
    }

    // Return a nullable vector3, representing the position of the player if it's in sight,
    // or null if it is out of sight
    protected Vector3? FindPlayer()
    {
        if (sightDistance == 0f)
        {
            return null; // if the sight distance is exactly 0, then we're blind and can never see the player
        }

        Vector2 dir = player.transform.position - transform.position;
        if (Vector2.Dot(dir.normalized, Heading) > 0.1f)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.37f, dir, sightDistance,
                LayerMask.GetMask("Friendly") | LayerMask.GetMask("Default") | LayerMask.GetMask("IgnoreProjectile"));
            if (hit.collider?.name == "Player")
            {
                return hit.collider.transform.position;
            }
        }
        return null;
    }

    public void StrikeMe(GameObject _projectile)
    {
        scoreCounter.Count += 1;
        //enemyCounter.Count -= 1;

        deathSound.Play();
        deathSound.transform.parent = transform.parent;
        Destroy(deathSound, deathSound.clip.length);

        if (Random.value < dropProbability && dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

        enemyCounter.Count -= 1;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Heading * Speed);
    }
}
