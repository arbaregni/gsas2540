using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerAggressiveEnemyBehavior : PlatformerEnemyBehavior
{
    [SerializeField]
    GameObject exclaim;

    [SerializeField]
    bool moveTowardsPlayer;

    [SerializeField]
    bool rangedAttack = true;

    [SerializeField]
    float fireInterval;

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    float projectileSpawnOffset = 2f;

    [SerializeField]
    bool meleeAttack = false;


    float lastFireTime = 0f;

    


    // Start is called before the first frame update
    void Start()
    {
        SetUp();

        if (hitbox != null)
        {
            hitbox.triggerEnterHandler = OnHitHandler;
        }

        StartCoroutine(State_Patrolling());
    }


    IEnumerator State_Patrolling()
    {

        while (true)
        {

            moving = true;
            {
                float stopAt = Time.time + patrolTime;
                while (Time.time < stopAt)
                {

                    if (FindPlayer().HasValue)
                    {
                        StartCoroutine(State_Pursuing());
                        yield break;
                    }

                    yield return null;

                }
            }
            moving = false;

            // turn around
            HeadingX = -HeadingX;

            {
                float stopAt = Time.time + pauseTime;
                while (Time.time < stopAt)
                {
                    if (FindPlayer().HasValue)
                    {
                        StartCoroutine(State_Pursuing());
                        yield break;
                    }

                    yield return null;
                }
            }



        }
    }

    IEnumerator State_Pursuing()
    {
        // let the player know we've seen them
        moving = false;
        exclaim.SetActive(true);
        yield return new WaitForSeconds(.5f);
        exclaim.SetActive(false);
        
        
        if (moveTowardsPlayer)
        {
            moving = true;
        }

        while (true)
        {
            Vector3? target = FindPlayer();
            if (!target.HasValue)
            {
                // lost the player:
                StartCoroutine(State_Patrolling());
                yield break;
            }

            Vector3 diff = target.Value - transform.position;

            if (rangedAttack && Time.time >= lastFireTime + fireInterval)
            {
                lastFireTime = Time.time;
                animator.SetTrigger("attack");
                Invoke(nameof(FireProjectile), .5f);
            }


            HeadingX = (int)Mathf.Sign(diff.x);

            yield return null;

        }


    }

    void FireProjectile()
    {
        Instantiate(projectilePrefab, transform.position + HeadingX * projectileSpawnOffset * Vector3.right, HeadingAsQuaternion);
    }

    void OnHitHandler(Collider2D collider)
    {
        if (!meleeAttack) return;

        PlatformerController player = collider.gameObject.GetComponent<PlatformerController>();

        if (player == null) return;

        player.StrikeMe(gameObject);
    }





    

}
