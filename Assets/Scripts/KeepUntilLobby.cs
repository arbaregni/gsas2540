using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// The attached GameObject isn't destroyed by any scene transitions, except for when we go back to the main menu
public class KeepUntilLobby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    // when we move to a new scene, we might want to change the music
    private void ActiveSceneChanged(Scene _arg0, Scene newScene)
    {
        if (newScene.buildIndex == 0)
        {
            // kill this when we go back to the main menu
            Destroy(this.gameObject);
        }
    }
}
