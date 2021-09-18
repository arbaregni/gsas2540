using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    private Toggle toggle;
    private bool isPaused;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // switch the pause state
            SetPause(!isPaused);
        }
    }

    public void SetPause(bool pause)
    {
        Debug.Log("set pause called");
        Time.timeScale = pause ? 0f : 1f;
        toggle.isOn = pause; // will this call itself?
        isPaused = pause;
    }

}
