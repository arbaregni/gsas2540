using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplePhysics : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * 10, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 normal = collision.GetContact(0).normal;
        Vector3 vel = Vector3.Reflect(rb.velocity, normal);
        rb.velocity = vel;
    }
}
