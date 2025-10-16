using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSize, vehicleSize;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;
    public int carCount;
    public GameObject carsSpace;
    public int realCarCount = 0;
    public List<GameObject> realCars;
    public WinningScript winScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnDrop(PointerEventData eventData) {
        Debug.Log("[INFO] OnDrop ZERO");
        if (eventData.pointerDrag!=null && Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Debug.Log("[INFO] OnDrop 1st If");
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
                vehicleZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
                Debug.Log("Rotation difference: " + rotDiff);
                placeSize = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                vehicleSize = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(vehicleSize.x - placeSize.x);
                ySizeDiff = Mathf.Abs(vehicleSize.y - placeSize.y);
                Debug.Log("X size difference: " + xSizeDiff);
                Debug.Log("Y size difference: " + ySizeDiff);
                if ((rotDiff <= 5) || (rotDiff>=355 && rotDiff<=360) && (xSizeDiff<=0.05 && ySizeDiff<=0.05))
                {
                    Debug.Log("Correct place");
                    objScript.rightPlace = true;
                    winScript.pointsAmount++;
                  
                    Debug.Log("Points: "+winScript.pointsAmount);
                    
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation = GetComponent<RectTransform>().localRotation;
                    eventData.pointerDrag.GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale;
                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            objScript.effects.PlayOneShot(objScript.audioCli[2]);
                            
                            break;

                        case "Medicine":
                            objScript.effects.PlayOneShot(objScript.audioCli[3]);
                            break;
                        case "Fire":
                            objScript.effects.PlayOneShot(objScript.audioCli[4]);
                            break;
                        case "School":
                            objScript.effects.PlayOneShot(objScript.audioCli[5]);
                            break;
                        case "B2":
                            objScript.effects.PlayOneShot(objScript.audioCli[6]);
                            break;
                        case "Cement":
                            objScript.effects.PlayOneShot(objScript.audioCli[7]);
                            break;
                        case "e46":
                            objScript.effects.PlayOneShot(objScript.audioCli[8]);
                            break;
                        case "e61":
                            objScript.effects.PlayOneShot(objScript.audioCli[9]);
                            break;
                        case "Excavator":
                            objScript.effects.PlayOneShot(objScript.audioCli[10]);
                            break;
                        case "Police":
                            objScript.effects.PlayOneShot(objScript.audioCli[11]);
                            break;
                        case "Tractor1":
                            objScript.effects.PlayOneShot(objScript.audioCli[12]);
                            break;
                        case "Tractor5":
                            objScript.effects.PlayOneShot(objScript.audioCli[13]);
                            break;
                        default:
                            Debug.Log("Unknown tag detected");

                            break;
                    }
                    
                    
                }
            }
            else
            {
                Debug.Log("Wrong place");
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[1]);
         
                bool found = false;
                for (int i = 0; i < realCars.Count; i++)
                {
                    if (realCars[i] != null && realCars[i].tag == eventData.pointerDrag.tag)
                    {
                        realCars[i].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[i];
                        Debug.Log(objScript.startCoordinates[i]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Debug.Log("Unknown tag detected");
                }
            }
        }
    }
    
    void Start()
    {
     
        carCount = objScript.vehicles.Length;
       
            int childCount = carsSpace.transform.childCount;
            
            for (int i = 0; i < childCount; i++)
            {
                realCars.Add(carsSpace.transform.GetChild(i).gameObject);
            }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carsSpace)
        {
            carCount = carsSpace.transform.childCount;
        }
    }
}
