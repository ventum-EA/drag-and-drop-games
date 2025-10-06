using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
public class ObstacleControllerScript : MonoBehaviour
{
    public float speed = 1f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    public float fadeDuration = 1.5f;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private Image image;
    private Color originalColor;
    private bool isExploding;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        originalColor = image.color;
        objectScript = Object.FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        if (speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80) && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }
        if (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }
        if (CompareTag("CloudBomb") && !isExploding && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("bomb hit by cursor");
            TriggerExplosion();
        }
        ///............
        
        if (ObjectScript.drag && !isFadingOut && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("Obstacle hit by drag");
            if (objectScript.lastDragged != null)
            {
                StartCoroutine(ShrinkAndDestroy(objectScript.lastDragged, 0.5f));
                objectScript.lastDragged = null;
                ObjectScript.drag = false;
            }
            if (CompareTag("CloudBomb"))
            {
                StartToDestroy(Color.red);

            }
            else
            {
                StartToDestroy(Color.cyan);
            }
            //StartCoroutine(FadeOutAndDestroy());
            //isFadingOut = true;
            //image.color = Color.cyan;
            //StartCoroutine(RecoverColor(.5f));
            //StartCoroutine(Vibrate());
            if (objectScript.effects != null && objectScript.audioCli != null)
            {
                objectScript.effects.PlayOneShot(objectScript.audioCli[13]);
            }


        }
    }

    IEnumerator FadeIn()
    {
        float a = 0f;
        while (a < fadeDuration)
        {
            a += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, a / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    IEnumerator FadeOutAndDestroy()
    {
        float a = 0f;
        float startAlpha = canvasGroup.alpha;
        while (a < fadeDuration)
        {
            a += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, a / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        Destroy(gameObject);
    }
    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.rotation;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0, 360, t / duration);
            target.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        Destroy(target);

    }
    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }
    IEnumerator Vibrate()
    {
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elapsed = 0f;
        float intensity = 5f;
        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void TriggerExplosion()
    {
        isExploding = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[14], 5f);
        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }
        image.color = Color.red;
        StartCoroutine(RecoverColor(.3f));
        StartCoroutine(Vibrate());
       StartCoroutine(WaitBeforeExplode());

    }
    IEnumerator WaitBeforeExplode()
    {
        float radius = 0;
        if(TryGetComponent<CircleCollider2D>(out CircleCollider2D circleCollider))
        {
            radius = circleCollider.radius * transform.lossyScale.x;
            
        }
        ExplodeAndDestroyNearbyObjects(radius);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
    public void StartToDestroy(Color c)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
            image.color = c;
            StartCoroutine(RecoverColor(.5f));
            StartCoroutine(Vibrate());
            objectScript.effects.PlayOneShot(objectScript.audioCli[13]);
        }
    }
    public void ExplodeAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach(Collider2D hit in hits)
        {
            if(hit != null && hit.gameObject != gameObject)
            {
                ObstacleControllerScript obj = hit.GetComponent<ObstacleControllerScript>();
                if(obj != null && !obj.isExploding)
                {
                    obj.StartToDestroy(Color.cyan);
                }
            }
        }
    }
}
