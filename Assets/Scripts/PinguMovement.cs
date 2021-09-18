using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinguMovement : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float dropInterval = 2f;

    [SerializeField]
    Vector3 direction = new Vector3(1f, 0f, 0f);

    [SerializeField]
    GameObject applePrefab;


    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        Invoke("CheckChangeDirection", 1.0f);
        Invoke("DropApple", dropInterval);
    }

    void CheckChangeDirection()
    {
        float angle = Random.Range(0f, 6.28f);
        direction = Mathf.Cos(angle) * Vector3.right + Mathf.Sin(angle) * Vector3.up;
        sprite.flipX = direction.x > 0;

        float time = Random.Range(0.5f, 2.0f);

        speed = Random.Range(0.5f, 10f);

        Invoke("CheckChangeDirection", time);
    }

    void DropApple()
    {
        Instantiate(applePrefab, transform.position, Quaternion.identity);

        Invoke("DropApple", dropInterval);

    }

    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (transform.position.x < -5 || 5 < transform.position.x)
        {
            direction = Vector3.Reflect(direction, Vector3.right);
            sprite.flipX = direction.x > 0;
            float x = Mathf.Clamp(transform.position.x, -5, 5);
            transform.position = new Vector3(x, transform.position.y, 0f);
        }
    }


}
