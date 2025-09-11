using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image buttonImg;
    public Color normalColor;
    public Color hoverColor = Color.gray;

    public Outline outline;
    public Color glowColor = Color.white;
    public float pulseSpeed = 2f;

    public float scaleRate = 1.2f;

    private Color ogGlowColor;
    private bool isHovered = false;
    private Vector3 originalScale;

    void Start()
    {
        if (outline != null)
        {
            ogGlowColor = outline.effectColor;

        }
        originalScale = transform.localScale;
        if (buttonImg != null)
        {
            ///set button color to the previously set color
        }
        normalColor = GetComponent<Image>().color;

    }

    // Update is called once per frame
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (buttonImg != null) buttonImg.color = hoverColor;
        transform.localScale = originalScale * scaleRate;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (buttonImg != null) buttonImg.color = normalColor;
        transform.localScale = originalScale;
    }
    void Update()
    {
        if(outline != null && isHovered)
        {
           //// float pulse
        }
    }
}
