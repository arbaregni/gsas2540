using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAScene :MonoBehaviour
{
    static public void ChangeScene(string name) 
    {
        SceneManager.LoadScene(name);
    }

    static public void ReturnToMain()
    {
        SceneManager.LoadScene("_MainMenu");
    }
}
