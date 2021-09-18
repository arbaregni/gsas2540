using System.Collections;
using UnityEngine;

public class PlatformerEnemyBehavior : MonoBehaviour
{
    [SerializeField]
    protected float patrolTime = 3f;

    [SerializeField]
    protected float pauseTime = 1f;

    [SerializeField]
    protected float speed = 4f;

    [SerializeField]
    protected float deathDelay = 1f;

    [SerializeField]
    protected float sightDistance;

    [SerializeField]
    protected Detector hitbox;


    protected bool moving;

    // positive when looking to the right, negative when looking to the left
    int _headingX = 1;
    public int HeadingX
    {
        get => _headingX;
        set /* ignores a value of zero */
        {
            if (value > 0)
            {
                spriteRenderer.flipX = false;
                hitbox.transform.localScale = new Vector3(+1, 1, 1);
            }
            if (value < 0)
            {
                spriteRenderer.flipX = true;
                hitbox.transform.localScale = new Vector3(-1, 1, 1);
            }
            if (value != 0)
            {
                _headingX = value;
            }
        }
    }

    public Quaternion HeadingAsQuaternion
    {
        get => Quaternion.LookRotation(Vector3.forward, HeadingX * Vector3.right);
    }



    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rb;

    static PlatformerController player = null;


    protected void SetUp()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        _headingX = spriteRenderer.flipX ? -1 : 1;

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<PlatformerController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    IEnumerator State_Patrol()
    {
        while (true)
        {
            moving = true;

            if (patrolTime != 0f)
            {
                // dont wait if it is exactly zero
                yield return new WaitForSeconds(patrolTime);
            }


            moving = false;

            HeadingX = -HeadingX;

            yield return new WaitForSeconds(pauseTime);

        }
    }

    void Update()
    {
        {
            float xspeed;
            if (Mathf.Abs(rb.velocity.y) >= 0.05)
            {
                xspeed = 0f;
            }
            else
            {
                xspeed = Mathf.Abs(rb.velocity.x);
            }
            animator.SetFloat("speed", xspeed);
        }


        {  /* check for death by falling */
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPoint.y <= 0)
            {
                // death by falling
                // TODO: juice

                Destroy(gameObject);
            }
        }

    }

    private void FixedUpdate()
    {
        if (moving)
        {
            rb.velocity = new Vector2(speed * HeadingX, rb.velocity.y);
        }

    }

    public void TakeHit()
    {
        animator.SetTrigger("die");
        StopAllCoroutines();
        DestroySelf();
    }

    void DestroySelf()
    {
        Invoke(nameof(_Destroy), deathDelay);
    }
    void _Destroy()
    {
        Destroy(gameObject);
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

        if (Mathf.Sign(dir.x) == HeadingX)
        {

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.37f, dir, sightDistance,
                LayerMask.GetMask("Friendly") | LayerMask.GetMask("Default"));

            if (hit.collider?.name == "Player")
            {
                return hit.collider.transform.position;
            }
        }
        return null;
    }

}
