using UnityEngine;

public class TransformationScript : MonoBehaviour
{
    public ObjectScript objScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objScript.lastDragged != null)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                objScript.lastDragged.GetComponent<Transform>().transform.Rotate(0, 0, Time.deltaTime * 15f);
            }
            if (Input.GetKey(KeyCode.X))
            {
                objScript.lastDragged.GetComponent<Transform>().transform.Rotate(0, 0, Time.deltaTime * -15f);
            }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                if (objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y < 1.2)
                {
                    objScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y + .01f, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.z);
                }
                }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y > 0.2)
                {
                    objScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y - .01f, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.z);
                }
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x <1.2)
                {
                    objScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x + .01f, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.z);
                }
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x > 0.2)
                {
                    objScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x - .01f, objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y , objScript.lastDragged.GetComponent<RectTransform>().transform.localScale.z);
                }
            }
        }
        } 
    }

