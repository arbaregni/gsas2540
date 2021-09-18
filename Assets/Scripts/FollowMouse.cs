using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        pos.x = Mathf.Clamp(-8, pos.x, 8);
        pos.y = transform.position.y;
        pos.z = transform.position.z;
        
        transform.position = pos;
    }

    /*
     we want to put N elements in. but we don't know N ahead of time.
    have a chunk of memory.

    minimize total size of allocations / N


     */
}
