using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [SerializeField]
    float coolDown = 0.2f;

    [SerializeField]
    GameObject projectilePrefab;


    private float lastFireTimeStamp;

    // Update is called once per frame
    void Update()
    {
        // look towards the mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        transform.up = (mousePos - transform.position).normalized;

        if (Input.GetMouseButtonUp(0) && Time.time - lastFireTimeStamp > coolDown)
        {
            lastFireTimeStamp = Time.time;
            GameObject proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }
}
