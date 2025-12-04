using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleControllerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    public float fadeDuration = 1.5f;

    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploding = false;
    private Image image;
    private Color orginalColor;

    // FIX: Variable to store the specific camera used for World Space UI
    private Camera worldCamera;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        orginalColor = image.color;

        objectScript = Object.FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBoundriesScript>();

        // FIX: Smart Camera Assignment
        // 1. Try to get the camera assigned to the Canvas
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null && rootCanvas.worldCamera != null)
        {
            worldCamera = rootCanvas.worldCamera;
        }
        // 2. Fallback to Main Camera if Canvas doesn't specify one
        else
        {
            worldCamera = Camera.main;
        }

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        // --- DESTROY LOGIC (Left/Right bounds) ---
        if (speed > 0 && transform.position.x < (screenBoundriesScript.worldBounds.min.x + 80) && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }

        if (speed < 0 && transform.position.x > (screenBoundriesScript.worldBounds.max.x - 80) && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }
        // ----------------------------------------

        Vector2 inputPosition;
        if (!TryGetInputPosition(out inputPosition))
            return;

        // FIX: SAFETY CHECK
        // If the object is somehow behind the camera, STOP. 
        // Calculating ScreenPoint on an object behind the camera causes the "Inf" error.
        if (!IsObjectInFrontOfCamera())
            return;

        // BOMB CLICK LOGIC
        if (CompareTag("CloudBomb") && !isExploding)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPosition, worldCamera))
            {
                Debug.Log("Bomb hit by cursor");
                TriggerExplosion();
            }
        }

        // DRAG COLLISION LOGIC
        if (ObjectScript.drag && !isFadingOut)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPosition, worldCamera))
            {
                Debug.Log("Obstacle hit by drag");
                if (objectScript.lastDragged != null)
                {
                    StartCoroutine(ShrinkAndDestroy(objectScript.lastDragged, 0.5f));
                    objectScript.lastDragged = null;
                    ObjectScript.drag = false;
                }

                if (CompareTag("CloudBomb"))
                    StartToDestroy(Color.red);
                else
                    StartToDestroy(Color.cyan);
            }
        }
    }

    // FIX: Helper method to prevent the crash
    private bool IsObjectInFrontOfCamera()
    {
        if (worldCamera == null) return false;

        // Convert the object's position to the camera's local space
        Vector3 viewPos = worldCamera.WorldToViewportPoint(transform.position);

        // If Z is negative, the object is behind the camera.
        // If X or Y are huge (infinity), it's too far away.
        return viewPos.z > 0 && viewPos.z < 1000;
    }

    bool TryGetInputPosition(out Vector2 position)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        position = Input.mousePosition;
        return true;

#elif UNITY_ANDROID
        if(Input.touchCount > 0)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            position = Vector2.zero;
            return false;
        }
#else
        position = Input.mousePosition;
        return true;
#endif
    }

    // ... [Rest of your code (TriggerExplosion, FadeIn, etc) remains exactly the same] ...

    public void TriggerExplosion()
    {
        isExploding = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[14], 5f);

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }
        if (objectScript.lastDragged && ObjectScript.drag)
        {
            StartCoroutine(ShrinkAndDestroy(objectScript.lastDragged, 0.5f));
            objectScript.lastDragged = null;
            ObjectScript.drag = false;
        }
        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));
        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());

    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circleCollider))
        {
            radius = circleCollider.radius * transform.lossyScale.x;

            yield return new WaitForSeconds(0.8f);
            ExploadAndDestroyNearbyObjects(radius);
            Destroy(gameObject);
        }
    }

    void ExploadAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject)
            {
                ObstacleControllerScript obj = hit.GetComponent<ObstacleControllerScript>();
                if (obj != null && !obj.isExploding)
                {
                    obj.StartToDestroy(Color.cyan);
                }
            }
        }
    }

    public void StartToDestroy(Color c)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            image.color = c;
            StartCoroutine(RecoverColor(0.5f));

            StartCoroutine(Vibrate());
            objectScript.effects.PlayOneShot(objectScript.audioCli[13]);
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
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, a / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 orginalScale = target.transform.localScale;
        // Quaternion orginalRotation = target.transform.rotation; // Unused variable removed
        float t = 0f;

        while (t < duration)
        {
            if (target == null) yield break;
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(orginalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0, 360, t / duration);
            target.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }
        if (target != null) Destroy(target);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (image != null) image.color = orginalColor;
    }

    IEnumerator Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
        if (rectTransform != null)
        {
            Vector2 orginalPosition = rectTransform.anchoredPosition;
            float duration = 0.3f;
            float elpased = 0f;
            float intensity = 5f;

            while (elpased < duration)
            {
                if (rectTransform == null) yield break;
                rectTransform.anchoredPosition = orginalPosition + Random.insideUnitCircle * intensity;
                elpased += Time.deltaTime;
                yield return null;
            }
        }
    }
}