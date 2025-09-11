using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSize, vehicleSize;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;  
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
                switch (eventData.pointerDrag.tag)
                {
                    case "Garbage":
                        objScript.vehicles[0].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[0];
                        break;

                    case "Medicine":
                        objScript.vehicles[1].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[1];
                        break;
                    case "Fire":
                        objScript.vehicles[2].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[2];
                        break;
                    case "School":
                        objScript.vehicles[3].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[3];
                        break;
                    case "B2":
                        objScript.vehicles[4].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[4];
                        break;
                    case "Cement":
                        objScript.vehicles[5].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[5];
                        break;
                    case "e46":
                        objScript.vehicles[6].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[6];
                        break;
                    case "e61":
                        objScript.vehicles[7].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[7];
                        break;
                    case "Excavator":
                        objScript.vehicles[8].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[8];
                        break;
                    case "Police":
                        objScript.vehicles[9].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[9];
                        break;
                    case "Tractor1":
                        objScript.vehicles[10].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[10];
                        break;
                    case "Tractor5":
                        objScript.vehicles[11].GetComponent<RectTransform>().localPosition = objScript.startCoordinates[11];
                        break;
                    default:
                        Debug.Log("Unknown tag detected");

                        break;
                }
            }
        }
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
