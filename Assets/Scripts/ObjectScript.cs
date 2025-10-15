using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public GameObject[] vehicles;
    [HideInInspector]
    public Vector2[] startCoordinates;
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector]
    public bool rightPlace = false;
    public GameObject lastDragged = null;
    public static bool drag = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        
        
    }
    void Start()
    {
        if (!vehicles.IsUnityNull())
        {
            startCoordinates = new Vector2[vehicles.Length];
            for (int i = 0; i < vehicles.Length; i++)
            {
                startCoordinates[i] = vehicles[i].GetComponent<RectTransform>().localPosition;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
