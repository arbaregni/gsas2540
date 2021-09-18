using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroyerBehavior : MonoBehaviour
{
    List<BombBehavior> bombList = new List<BombBehavior>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BlowUpBombs()
    {
        // first stop them all from falling
        foreach (BombBehavior bomb in bombList)
        {
           //  bomb.Stop();
        }
    }
}
