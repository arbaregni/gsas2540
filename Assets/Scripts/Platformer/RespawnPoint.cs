using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Detector))]
public class RespawnPoint : MonoBehaviour
{
    bool isActivated;

    // Start is called before the first frame update
    void Start()
    {
        isActivated = false;
        Detector detector = GetComponent<Detector>();
        detector.triggerEnterHandler = SetRespawn;
    }

    void SetRespawn(Collider2D collider)
    {
        if (isActivated) return;

        PlatformerController player = collider.gameObject.GetComponent<PlatformerController>();

        if (player == null) return;

        player.RespawnPoint = transform.position;

        isActivated = true;
    }

}
