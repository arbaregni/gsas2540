using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField]
    Sprite activatedSprite;

    [SerializeField]
    AudioSource activatedSound;

    [SerializeField]
    Detector detector;

    [SerializeField]
    Animator toTrigger;

    [SerializeField]
    string triggerName;

    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        detector.triggerEnterHandler = OnDetectPlayer;
    }

    void OnDetectPlayer(Collider2D other)
    {
        if (activated) return;

        var player = other.gameObject.GetComponent<PlatformerController>();
        if (player == null) return;

        GetComponentInChildren<SpriteRenderer>().sprite = activatedSprite;
        activatedSound.Play();

        activated = true;

        var cam = GameObject.Find("Focus").GetComponent<PlatformerCameraController>();

        // have the camera pan to the animator's position,
        // then we start the animation, pause for a second, and then come back
        cam.HighlightPosition(toTrigger.gameObject.transform.position, 1f, DoSetTrigger);
    }

    void DoSetTrigger()
    {
        toTrigger.SetTrigger(triggerName);
    }
}
