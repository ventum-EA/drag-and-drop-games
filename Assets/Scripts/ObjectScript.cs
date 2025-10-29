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
    public DropPlaceScript dropPlaceScript;
    // Start is called once before the first execution of Upd   ate after the MonoBehaviour is created

    void Awake()
    {
        
        
    }
    void Start()
    {
        if (!dropPlaceScript.realCars.IsUnityNull())
        {
            startCoordinates = new Vector2[dropPlaceScript.realCars.Count];
            for (int i = 0; i < dropPlaceScript.realCars.Count; i++)
            {
                startCoordinates[i] = dropPlaceScript.realCars[i].GetComponent<RectTransform>().localPosition;
            }
        }
        Debug.Log(startCoordinates);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
