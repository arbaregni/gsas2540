using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float initialVelocity;
    public float lifetime = 100f;

    [SerializeField]
    AudioSource birthSound;

    [SerializeField]
    AudioSource defaultHitSound;

    [SerializeField]
    float fadeOutTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        birthSound.Play();
        GetComponent<Rigidbody2D>().velocity = transform.up * initialVelocity;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IStrikable strikable = collision.gameObject.GetComponent<IStrikable>();
        if (strikable != null)
        {
            strikable.StrikeMe(gameObject);
            Destroy(gameObject);
        }
        else
        {
            defaultHitSound.Play();
            Destroy(GetComponent<Rigidbody2D>());
            StartCoroutine(Disappear(fadeOutTime));
        }
    }

    IEnumerator Disappear(float span)
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        float start = Time.time;
        while (Time.time - start < span)
        {
            float t = (Time.time - start) / span;
            Color col = spriteRenderer.color;
            col.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = col;
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }
}
