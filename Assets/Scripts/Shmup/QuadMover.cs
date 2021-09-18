using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quad movers can move up/down and left/right and have moving and idle animations to reflect aht
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class QuadMover : MonoBehaviour
{
    [SerializeField]
    protected float fullSpeed = 5f;

    protected Animator animator;
    protected Rigidbody2D rb;

    protected int horizontality = 0;
    protected int verticality = 1;

    public Vector2 Heading
    {
        get => new Vector2(horizontality, verticality).normalized;
    }
    public Quaternion HeadingAsQuaternion
    {
        get => Quaternion.LookRotation(Vector3.forward, Heading);
    }

    public float Speed
    {
        get; protected set;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Speed = fullSpeed;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (horizontality != 0 || verticality != 0)
        {
            animator.SetInteger("Horizontality", horizontality);
            animator.SetInteger("Verticality", verticality);
        }
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        rb.velocity = Heading * Speed;
    }
}
