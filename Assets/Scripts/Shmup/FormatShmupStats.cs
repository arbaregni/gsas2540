using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Text))]
public class FormatShmupStats : MonoBehaviour
{
    [SerializeField]
    string shmupPlayerStats;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        UpdateStats();
    }

    private void OnActiveSceneChanged(Scene _arg0, Scene newScene)
    {
        if (newScene.name == "Shmup_GameOver")
        {
            UpdateStats();
        }
    }

    void UpdateStats()
    {
        Debug.Log("updating stats....");
        Shmup_PlayerStats stats = GameObject.Find("ShmupPlayerStats")?.GetComponent<Shmup_PlayerStats>();
        if (stats == null)
        {
            Debug.LogWarning("Missing 'ShmupPlayerStats' object with correct component");
            return;
        }
        string template = GetComponent<Text>().text;
        GetComponent<Text>().text = stats.FormatStats(template,
            // say "NEW RECORD!" in rainbow text
            "<color=red>N</color><color=orange>E</color><color=yellow>W</color> <color=lime>R</color><color=#008000>E</color><color=cyan>C</color><color=blue>O</color><color=#4b0082>R</color><color=#ee82ee>D</color><color=#f54284>!</color>");
    }


}
