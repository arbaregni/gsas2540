using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTracker : MonoBehaviour
{
    [SerializeField]
    GameObject iconPrefab;

    [SerializeField]
    float xSpacing;

    [SerializeField]
    int maxCount;

    private GameObject[] icons;

    private int _count; // the icons from index 0 (inclusive) up to _count (exclusive) are turned on
    public int Count
    {
        get => _count;
        set
        {
            
            if (_count != value)
            {
                for (int i = 0; i < icons.Length; ++i)
                {
                    icons[i].SetActive(i < value);
                }
            }
            _count = value;
            if (_count > maxCount)
                _count = maxCount;
            if (_count < 0)
                _count = 0;
        }
    }

    public int MaxCount { get => maxCount; }
        
    // Start is called before the first frame update
    void Awake()
    {
        icons = new GameObject[maxCount];
        for (int i = 0; i < maxCount; ++i)
        {
            icons[i] = Instantiate(iconPrefab, transform);
            icons[i].transform.position += xSpacing * i * Vector3.left;
        }
    }
}
