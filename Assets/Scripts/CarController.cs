using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    float rotSpeed = -30f;


    [SerializeField]
    Transform[] steeringWheels;

    Vector2 inputAxes;
    float steeringAngle;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnAlertHandler()
    {
        Debug.Log("received alert event!");
    }

    // Update is called once per frame
    void Update()
    {
        inputAxes.x = Input.GetAxis("Horizontal");
        inputAxes.y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        steeringAngle += inputAxes.x * rotSpeed * Time.deltaTime;
        steeringAngle = Mathf.Clamp(steeringAngle, -30f, 30f);

        foreach (Transform wheel in steeringWheels)
        {
            wheel.localRotation = Quaternion.Euler(0f, 0f, steeringAngle);
        }
        // 
        if (inputAxes.y > .01f) {
            float step = 2.5f * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.up, steeringWheels[0].transform.up, step, 0.0f);
            Debug.DrawRay(transform.position, newDir);
            transform.up = newDir;
        }
        // transform.ro;

        // rb.velocity = transform.up * inputAxes.y * speed;
    }
    
}
