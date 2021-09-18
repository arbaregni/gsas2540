using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VolumeIndicatorBehavior : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprites;

    [SerializeField]
    float[] volumeLevels;

    [SerializeField]
    int initialState = 2;

    int numStates;
    int state;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();

        if (sprites.Length != volumeLevels.Length)
        {
            Debug.LogError($"volume indicator sprites.Length ({sprites.Length}) is not equal to volumeLevels.Length ({volumeLevels.Length})");
        }
        numStates = Mathf.Min(sprites.Length, volumeLevels.Length);
        state = initialState;
        UpdateState();
    }

    public void OnClick()
    {
        state = (state + 1) % numStates;
        UpdateState();
    }

    void UpdateState()
    {
        image.sprite = sprites[state];
        AudioManager.Instance.SetVolume(volumeLevels[state]);
    }
}
