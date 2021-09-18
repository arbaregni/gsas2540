using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCameraController : MonoBehaviour
{
    [SerializeField]
    Transform tracking;

    [SerializeField]
    Bounds bounds;

    [SerializeField]
    float maxSpeed = 100f;

    [SerializeField]
    Transform parallaxedBackground;

    [SerializeField]
    float parallaxMultiplier = -1f;


    enum CameraState
    {
        LOCKED_ON_PLAYER, // following player,
        EASING, // moving from place to place
        HIGHLIGHTING, // focused on something other than player
    }
    CameraState state;

    void Start()
    {
        state = CameraState.LOCKED_ON_PLAYER;
    }

    private void Update()
    {
        if (state == CameraState.LOCKED_ON_PLAYER)
        {

            Vector3 pos = transform.position;
            pos.x = tracking.position.x;
            pos.y = tracking.position.y;
            transform.position = ClampToBounds(pos);

        }

        {
            Vector3 pos = parallaxedBackground.position;
            pos.x = transform.position.x * parallaxMultiplier;
            parallaxedBackground.position = pos;
        }
        

    }

    // true iff the camera is doing something special, like easing to a new location
    public bool IsCinematic
    {
        get => state != CameraState.LOCKED_ON_PLAYER;
    }


    public void EaseTo(Vector3 dest)
    {
        StartCoroutine(EaseTo_Cortn(dest));
    }
    IEnumerator EaseTo_Cortn(Vector3 dest)
    {
        state = CameraState.EASING;

        dest = ClampToBounds(dest);

        Vector3 init = transform.position;
        float dist = (dest - init).magnitude;
        float duration = dist / maxSpeed;

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            yield return null;

            float t = (Time.time - startTime) / duration;

            // ease in and ease out
            // NOTE: at t = .5 this function achieves it's maximum speed of 1.5, so we'll actually go a little faster than the max speed
            transform.position = Vector3.Lerp(init, dest, t * t * (3 - 2 * t));
        }

        state = CameraState.LOCKED_ON_PLAYER;

        yield break;
    }

    public delegate void CameraArrivalCallback();

    // does a zelda style hey look at this position then comes back
    public void HighlightPosition(Vector3 target, float waitAtTarget, CameraArrivalCallback callback)
    {
        StartCoroutine(HighlightPosition_Cortn(target, waitAtTarget, callback));
    }
    IEnumerator HighlightPosition_Cortn(Vector3 target, float waitAtTarget, CameraArrivalCallback callback)
    {
        Vector3 init = transform.position;
        
        yield return EaseTo_Cortn(target);
        
        state = CameraState.HIGHLIGHTING;
        callback.Invoke();
        yield return new WaitForSeconds(waitAtTarget);
        
        yield return EaseTo_Cortn(init);

        state = CameraState.LOCKED_ON_PLAYER;

        yield break;
    }

    Vector3 ClampToBounds(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);
        return pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
