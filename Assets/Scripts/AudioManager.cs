using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    public static AudioManager Instance { get => instance; }

    // the current audio source that is playing
    [SerializeField]
    AudioSource currAudio = null;

    // the values (that we control) of currAudio
    [SerializeField] bool loop = true;
    [SerializeField] float volume = 0.5f;
    [SerializeField] bool isPlaying = true;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }
    public void Start()
    {
        SetVolume(volume);
    }

    public void SetVolume(float value)
    {
        volume = value / 10f;
        UpdateCurrAudioValues();
    }
    public void TogglePlaying(bool value)
    {
        isPlaying = value;
        UpdateCurrAudioValues();
    }

    // when the current audio source or one of the parameters changes,
    // call this function to match everything up
    private void UpdateCurrAudioValues()
    {
        if (currAudio == null) return;
        currAudio.volume = volume;
        currAudio.loop = loop;
        if (isPlaying)
        {
            currAudio.UnPause();
        }
        else if (currAudio.isPlaying)
        {
            currAudio.Pause();
        }
    }
}
