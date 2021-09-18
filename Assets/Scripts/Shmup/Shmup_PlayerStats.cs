using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shmup_PlayerStats : MonoBehaviour
{
    int shotsFired;
    
    int killCount;
    int highestKillCount;
    bool newHighestKills = false;

    public float Accuracy { get => (shotsFired == 0)? 0f : (float)killCount / shotsFired; }
    float bestAccuracy;
    bool newBestAccuracy;

    float startTime;
    float endTime;
    public float TimeElapsed { get => endTime - startTime; }
    float longestTime;
    bool newLongestTime = false;

    int wavesSurvived;
    int mostWavesSurvived;
    bool newMostWavesSurvived = false;
    
    ShmupController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<ShmupController>();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        ShmupController.OnGameOverEvent += OnGameOver;

        OnGameStart();
    }

    void OnGameStart()
    {
        startTime = Time.time;
        newHighestKills = false;
        newBestAccuracy = false;
        newLongestTime = false;
        newMostWavesSurvived = false;
    }

    private void OnActiveSceneChanged(Scene _arg0, Scene newScene)
    {
        if (newScene.name == "_ShootEmUp")
        {
            OnGameStart();
        }
    }

    void OnGameOver()
    {
        // update our metrics
        {
            endTime = Time.time;
            shotsFired = player.ShotsFired;
            killCount = GameObject.Find("ScoreText").GetComponent<TextCounter>().Count;
            wavesSurvived = GameObject.Find("WaveSummoner").GetComponent<WaveSummonerScript>().WaveCount;
        }

        // check the high scores
        {
            highestKillCount = PlayerPrefs.GetInt("HighestKills", 0);
            if (killCount > highestKillCount)
            {
                newHighestKills = true;
                PlayerPrefs.SetInt("HighestKills", killCount);
            }

            bestAccuracy = PlayerPrefs.GetFloat("BestAccuracy", 0f);
            if (Accuracy > bestAccuracy)
            {
                newBestAccuracy = true;
                PlayerPrefs.SetFloat("BestAccuracy", Accuracy);
            }

            longestTime = PlayerPrefs.GetFloat("LongestTime", 0f);
            if (TimeElapsed > longestTime)
            {
                newLongestTime = true;
                PlayerPrefs.SetFloat("LongestTime", TimeElapsed);
            }

            mostWavesSurvived = PlayerPrefs.GetInt("MostWavesSurvived", 0);
            if (wavesSurvived > mostWavesSurvived)
            {
                newMostWavesSurvived = true;
                PlayerPrefs.SetInt("MostWavesSurvived", wavesSurvived);
            }
        }
    }

    public string FormatStats(string template, string congrats)
    {
        string FormatDuration(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            if (seconds >= 3600f) // played at least an hour
            {
                return string.Format("{0}h:{1}m:{2}s", time.Hours.ToString(), time.Minutes.ToString(), time.Seconds.ToString());
            }
            if (seconds >= 60f) // played at least a minute
            {
                return string.Format("{0}m:{1}s", time.Minutes.ToString(), time.Seconds.ToString());
            }
            // played under a minute
            return string.Format("{0}s", time.Seconds.ToString());
        }
       
        return template
            .Replace("{killCount}", killCount.ToString())
            .Replace("{highestKillCount}", highestKillCount.ToString())
            .Replace("{newHighestKills}", newHighestKills? congrats : "")
            .Replace("{accuracy}", Mathf.Round(100 * Accuracy).ToString())
            .Replace("{bestAccuracy}", Mathf.Round(100 * bestAccuracy).ToString())
            .Replace("{newBestAccuracy}", newBestAccuracy? congrats : "")
            .Replace("{time}", FormatDuration(TimeElapsed))
            .Replace("{longestTime}", FormatDuration(longestTime))
            .Replace("{newLongestTime}", newLongestTime? congrats : "")
            .Replace("{wavesSurvived}", wavesSurvived.ToString())
            .Replace("{mostWavesSurvived}", mostWavesSurvived.ToString())
            .Replace("{newMostWavesSurvived}", newMostWavesSurvived? congrats: "");
    }


}
