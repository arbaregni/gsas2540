using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerController : MonoBehaviour, IStrikable
{
    [SerializeField]
    float speed;

    [SerializeField]
    float airSpeed;

    [SerializeField]
    float jumpForce;

    [SerializeField]
    float coyoteTime;

    [SerializeField]
    float pushAmount;

    [SerializeField]
    float dragCoeff = 5f;

    [SerializeField]
    float fastFallCoeff = 1f;

    [SerializeField]
    Detector leftSide;

    [SerializeField]
    Detector rightSide;

    [SerializeField]
    Detector feets;

    [SerializeField]
    Detector overHead;

    [SerializeField]
    Detector weaponHitBox;

    [SerializeField]
    AudioSource weaponSound;

    [SerializeField]
    AudioSource onHitSound;

    [SerializeField]
    AudioSource onFallSound;

    [SerializeField]
    AudioSource onDeathSound;


    private int currJumps;
    [SerializeField]
    int totalNumJumps = 1;


    Vector2 _respawnPoint;
    public Vector2 RespawnPoint { get => _respawnPoint; set => _respawnPoint = value; }

    Vector2 inputVector;
    Vector2Int inputVectorRaw;

    float yKillThreshold = float.NegativeInfinity; // kill the player if they fall below this

    float clingBeginTime; // the time when we first started clinging
    float coyoteTimerStart; // the last time when we are allowed to jump

    bool grounded;
    bool clinging;

    bool jump;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    PlatformerCameraController camController;
    public bool IsAttacking
    {
        get => animator.GetCurrentAnimatorStateInfo(0).IsName("BigSwing");
    }

    List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();

    // positive when pointing right, negative when pointing left.
    // will never be zero
    int _headingX = 1;
    public int HeadingX
    {
        get => _headingX;
        set /* ignores a value of zero */
        {
            if (value > 0)
            {
                spriteRenderer.flipX = false;
                weaponHitBox.transform.localScale = new Vector3(+1, 1, 1);
            }
            if (value < 0)
            {
                spriteRenderer.flipX = true;
                weaponHitBox.transform.localScale = new Vector3(-1, 1, 1);
            }
            if (value != 0)
            {
                _headingX = value;
            }
        }
    }

   

    // Start is called before the first frame update
    void Start()
    {
        // initial respawn is wherever we started in the scene
        RespawnPoint = transform.position;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        camController = GameObject.Find("Focus").GetComponent<PlatformerCameraController>();
        weaponHitBox.triggerEnterHandler = HitEnemy;

        feets.triggerEnterHandler = OnFeetsDown;
        feets.triggerExitHandler = OnFeetsUp;

        yKillThreshold = Camera.main.ViewportToWorldPoint(new Vector3(0, 0)).y;

        
    }

    // parent ourselves to whatever collider we may be standing upon
    void OnFeetsDown(Collider2D collider)
    {
        transform.parent = collider.gameObject.transform;
    }
    // unparent when we leave
    void OnFeetsUp(Collider2D _collider)
    {
        transform.parent = transform.root;
    }


    // Update is called once per frame
    void Update()
    {
        // update the animator and our selves with what state the character is in
        //  are they grounded?
        //  are they able to cling to a wall?
        {
            grounded = feets.HasContacts;
            clinging = false;
            if (feets.IsEmpty)
            {
                // if we're not grounded we could be hanging onto walls, but not when pressing down cause that causes us to slip off
                if (inputVector.y != -1 && (leftSide.HasContacts ^ rightSide.HasContacts))
                {
                    clingBeginTime = Time.time;
                    clinging = true;

                    rb.velocity = Vector2.zero;

                    if (leftSide.HasContacts)  HeadingX = -1;
                    if (rightSide.HasContacts) HeadingX =  1;
                }
            }
        }

        // update the animation parameters
        animator.SetFloat("speed", rb.velocity.magnitude);
        animator.SetBool("clinging", clinging);
        animator.SetBool("grounded", grounded);

        // keep track of the duration of our states for coyote time
        if (grounded || clinging)
        {
            currJumps = totalNumJumps;
            coyoteTimerStart = Time.time;
        }

        weaponHitBox.gameObject.SetActive(IsAttacking);


        if (IsAttacking || camController.IsCinematic)
        {
            // simply wait for the animation to finish: don't allow user input to move the character
        }
        else
        {
            // get the raw and adjusted input axes
            inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2Int previousInputVectorRaw = inputVectorRaw;
            inputVectorRaw = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));

            if (!clinging && inputVectorRaw.x != 0)
            {
                // tapping the X axis key should flip our sprite to look in the right direction
                HeadingX = inputVectorRaw.x;
            }

            // check for the possibility of a jump: first we need to be sure that we have enough jumps, and we only do it when the jump button is pressed down
            if (currJumps > 0 && inputVectorRaw.y == 1 && previousInputVectorRaw.y != 1)
            {
                bool doJump = false;

                // the user is inputting a jump
                if (clinging)
                {

                    doJump = true;
                    
                    if (overHead.IsEmpty)
                    {
                        doJump = true; // we can jump over a ledge
                    }
                    if (inputVectorRaw.x == -HeadingX)
                    {
                        doJump = true; // we can push off from a ledge when the directional input is away from the wall
                    }
                }

                else /* not clinging */
                {

                    // check coyote time
                    if (Time.time - coyoteTimerStart <= coyoteTime)
                    {
                        doJump = true;
                    }

                }


                if (doJump)
                {
                    currJumps -= 1;
                    jump = true;

                    // TODO: juice
                }
            }


            if (grounded && Input.GetButtonDown("Attack"))
            {
                weaponSound.Play();
                animator.SetTrigger("attack");
            }

            if (clinging && inputVectorRaw.y == -1)
            {
                // stop clinging
                clinging = false;
            }


        }

        /* check for death by falling */
        if (transform.position.y < yKillThreshold)
        {
            onFallSound.Play();
            DieAndRespawn();
        }
    }

    private void FixedUpdate()
    {
        if (clinging)
        {
            Vector2 holdingForce = -Physics2D.gravity * rb.mass * rb.gravityScale + Vector2.down * 0.2f;
            rb.AddForce(holdingForce, ForceMode2D.Force); //TODO this force decreases over time?


            // check that we only push off away from a wall, not into one
            if (inputVectorRaw.x == -HeadingX || jump)
            {
                Vector2 pushForce = Vector2.right * -HeadingX * pushAmount;
                rb.AddForce(pushForce, ForceMode2D.Impulse);
                clinging = false;   
            }

        }
        else
        {
            float s = grounded ? speed : airSpeed;

            if (inputVectorRaw.x == 0 || IsAttacking)
            {
                Vector2 dragForce = -1 * dragCoeff * rb.velocity.x * Vector2.right;
                rb.AddForce(dragForce, ForceMode2D.Force);
            }
            else if (inputVectorRaw.x != 0)
            {
                rb.velocity = new Vector2(inputVector.x * s, rb.velocity.y);
            }

            if (!grounded && inputVectorRaw.y == -1)
            {
                Vector2 fastFallForce = fastFallCoeff * Vector2.down;
                rb.AddForce(fastFallForce, ForceMode2D.Force);
            }
        }

        if (jump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }        
    }

    void HitEnemy(Collider2D other)
    {
        PlatformerEnemyBehavior enemy = other.GetComponent<PlatformerEnemyBehavior>();
        if (enemy == null) return;

        onHitSound.Play();
        enemy.TakeHit();
    }

    public void StrikeMe(GameObject projectile)
    {
        // play a death sound
        onDeathSound.Play();
        DieAndRespawn();
    }
    void DieAndRespawn()
    {
        camController.EaseTo(RespawnPoint);
        transform.position = RespawnPoint;
        rb.velocity = Vector2.zero;
        inputVector = Vector3.zero;
        inputVectorRaw = Vector2Int.zero;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        float radius = .025f;
        foreach (ContactPoint2D cp in contactPoints)
        {
            Gizmos.DrawSphere(cp.point, radius);
        }
    }

    
}
