using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField]
    float avgFlickerIntensity;

    [SerializeField]
    float maxIntensity;

    [SerializeField]
    float avgFlickerLength;

    [SerializeField]
    float maxFlickerLength;

    Light2D light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();

        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float intensity = -Mathf.Log(Random.value) / avgFlickerIntensity; // exponential distribution
            intensity = Mathf.Clamp(intensity, 0f, maxIntensity);

            light.intensity = intensity;

            float duration = -Mathf.Log(Random.value) / avgFlickerIntensity;
            duration = Mathf.Clamp(duration, 0f, maxFlickerLength);

            yield return new WaitForSeconds(duration);

        }



    }
}
