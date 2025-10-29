using UnityEngine;
using UnityEngine.EventSystems;

public class TransformationScript : MonoBehaviour
{
    public ObjectScript objScript;
    public float rotationSpeed = 90f;
    public float scaleSpeed = 0.5f;
    private bool rotateCW, rotateCCW, scaleUp, scaleDown, scaleLeft, scaleRight;
    public static bool isTransforming = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objScript.lastDragged == null)
        {
            return;
        }
        RectTransform rt = objScript.lastDragged.GetComponent<RectTransform>();
        if(rotateCW)
            rt.Rotate(0,0,-rotationSpeed * Time.deltaTime);
        if(rotateCCW)
            rt.Rotate(0,0,rotationSpeed * Time.deltaTime);
        if(scaleUp && rt.localScale.y<0.8f)
            rt.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        if(scaleDown && rt.localScale.y>0.35f)
            rt.localScale -= new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        if(scaleLeft && rt.localScale.x>0.35f)
            rt.localScale -= new Vector3(scaleSpeed * Time.deltaTime, 0, 0);
        if(scaleRight && rt.localScale.x<0.8f)
            rt.localScale += new Vector3(scaleSpeed * Time.deltaTime, 0, 0);
        isTransforming = rotateCW || rotateCCW || scaleUp || scaleDown || scaleLeft || scaleRight;


    }
    public void StartRotateCW(BaseEventData data){rotateCW = true;}
    public void StopRotateCW(BaseEventData data){rotateCW = false; }
    public void StartRotateCCW(BaseEventData data){rotateCCW = true; }
    public void StopRotateCCW(BaseEventData data){rotateCCW = false; }
    public void StartScaleUp(BaseEventData data){scaleUp = true; }
    public void StopScaleUp(BaseEventData data){scaleUp = false; }
    public void StartScaleDown(BaseEventData data){scaleDown = true; }
    public void StopScaleDown(BaseEventData data){scaleDown = false; }
    public void StartScaleLeft(BaseEventData data){scaleLeft = true; }
    public void StopScaleLeft(BaseEventData data){scaleLeft = false; }
    public void StartScaleRight(BaseEventData data){scaleRight = true; }
    public void StopScaleRight(BaseEventData data){scaleRight = false; }


}

