using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMask;

    public delegate void DetectObjectDelegate(Collider2D collision);

    [SerializeField]
    public DetectObjectDelegate triggerEnterHandler = null;

    [SerializeField]
    public DetectObjectDelegate triggerExitHandler = null;

    int count = 0;

    bool GameObjectInLayer(GameObject gameObject)
    {
        return layerMask.value == (layerMask.value | (1 << gameObject.layer));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameObjectInLayer(collision.gameObject))
        {
            count += 1;
            triggerEnterHandler?.Invoke(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameObjectInLayer(collision.gameObject))
        {
            count -= 1;
            triggerExitHandler?.Invoke(collision);
        }
    }

    public bool IsEmpty { get => count == 0; }

    public bool HasContacts { get => !IsEmpty; }

}
