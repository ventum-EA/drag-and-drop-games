using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerDownHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;

    private Vector3 dragOffsetWorld;
    private Camera uiCamera;
    private Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGro = GetComponent<CanvasGroup>();   
        rectTra = GetComponent<RectTransform>();

    }
    public void OnPointerDown(PointerEventData eventData)
    {

            Debug.Log("OnPointerDown");
            objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
        
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
  
            ObjectScript.drag = true;
            objectScr.lastDragged = null;
            canvasGro.blocksRaycasts = false;
            canvasGro.alpha = 0.6f;
            // rectTra.SetAsLastSibling();
            int positionIndex = transform.parent.childCount - 2;
            transform.SetSiblingIndex(positionIndex);

        Vector3 pointerWorld;
        if(ScreenPointToWorld(eventData.position, out pointerWorld))
        {
            dragOffsetWorld = rectTra.position - pointerWorld;
        }
        else
        {
            dragOffsetWorld = Vector3.zero;
        }
        objectScr.lastDragged = eventData.pointerDrag;

    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pointerWorld;
        if (!ScreenPointToWorld(eventData.position, out pointerWorld))
            return;
        Vector3 desired = pointerWorld + dragOffsetWorld;
        desired.z = rectTra.position.z;
        screenBou.RecalculateBounds();

        Vector2 clamped = screenBou.GetClampedPosition(desired);
        transform.position = new Vector3(clamped.x, clamped.y, desired.z);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            ObjectScript.drag = false;
            canvasGro.blocksRaycasts = true;
            canvasGro.alpha = 1.0f;
            if(objectScr.rightPlace)
            {
                canvasGro.blocksRaycasts = false;
                objectScr.lastDragged = null;
            }
            objectScr.rightPlace = false;
        }
    }
            
    // Update is called once per frame - 2 frames - 3 frames - 4 frames - 5 frames - 6 frames
    void Update()
    {
        
    }
    void Awake()
    {
        if (objectScr == null)
        {
            objectScr = Object.FindFirstObjectByType<ObjectScript>();

        }
        if(screenBou == null)
        {
            screenBou = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        }
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;
        }
    }

    private bool ScreenPointToWorld(Vector2 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;
        if (uiCamera != null)
            return false;
        float z = Mathf.Abs(uiCamera.transform.position.z - rectTra.position.z);
        Vector3 sp = new Vector3(screenPoint.x, screenPoint.y, z);
        worldPoint = uiCamera.ScreenToWorldPoint(sp);   
        return true;
    }
}
