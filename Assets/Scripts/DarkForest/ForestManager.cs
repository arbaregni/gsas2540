using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestManager : MonoBehaviour
{
    // control the bounds of the world
    [SerializeField]
    int minX;
    [SerializeField]
    int maxX;
    [SerializeField]
    int minY;
    [SerializeField]
    int maxY;

    // put all the audio here why not
    public AudioSource onMissAudio;
    public AudioSource onHitAudio;
    public AudioSource outOfBoundsAudio;
    public AudioSource onWalkAudio;
    public AudioSource backgroundMusic;

    // the distance that marks right next to (for axe hits)
    public float adjacentDist = 1.1f;
    // the distance that marks somewhere near by
    public float nearDist = 2f;
    public float farDist = 4f;

    float _audioWait = 2;
    public float AudioWait
    {
        get => _audioWait;
    }

    private static ForestManager _instance = null;
    public static ForestManager Instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this) // already initalized & doesn't match
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        _audioWait = Math.Max(
            onMissAudio.clip.length,
            Math.Max(
               onHitAudio.clip.length,
               outOfBoundsAudio.clip.length
            )
        );
    }
    public void ToggleBackgroundMusic(bool value)
    {
        Debug.Log($"toggling background music, value = {value}");

        /*AudioSource[] all = GameObject.FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in all)
        {
            Debug.Log(src);
        }
        */

        // otherwise, pause or play as requested
        if (value)
        {
            backgroundMusic.Play();
        }
        else
        {
            backgroundMusic.Pause();
        }
    }

    public bool InBounds(Vector3 pos)
    {
        return minX <= pos.x && pos.x <= maxX
            && minY <= pos.y && pos.y <= maxY;
    }

    public bool AudioIsPlaying()
    {
        return onMissAudio.isPlaying
            || onHitAudio.isPlaying
            || onWalkAudio.isPlaying
            || outOfBoundsAudio.isPlaying;
    }

    // returns a volume from 0 to 1, where the smaller the distance the louder the volume
    public static float ScaleVolume(float dist)
    {
        return 1.0f - Mathf.Lerp(0.0f, 0.9f, 0.25f * dist);
    }
}
