using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Detector))]
public class GameEnder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var detector = GetComponent<Detector>();
        detector.triggerEnterHandler = DoEndGame;
    }

    void DoEndGame(Collider2D other)
    {
        // load up some stats in a scriptable object????


        SceneManager.LoadScene("Platformer_GameOver");
    }
}
