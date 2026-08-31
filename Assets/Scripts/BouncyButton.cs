using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class BouncyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover & Press Animation Settings")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressedScale = 0.94f;
    [SerializeField] private float pressedOffsetY = -6f;
    [SerializeField] private float springSpeed = 18f;

    [Header("Idle Pulse / Breathing Effect")]
    [SerializeField] private bool enableIdleBreathing = false;
    [SerializeField] private float breathingScaleAmount = 0.03f;
    [SerializeField] private float breathingSpeed = 2.4f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Vector3 targetScale;
    private Vector2 targetPosition;
    private Vector3 scaleVelocity;
    private Vector2 posVelocity;
    private bool isHovered = false;
    private bool isPressed = false;
    private float breathingTimer = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        targetScale = originalScale;
        targetPosition = originalPosition;
        breathingTimer = Random.Range(0f, Mathf.PI * 2f);
    }

    private void OnEnable()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
            rectTransform.anchoredPosition = originalPosition;
            targetScale = originalScale;
            targetPosition = originalPosition;
            scaleVelocity = Vector3.zero;
            posVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        Vector3 desiredScale = targetScale;
        Vector2 desiredPos = targetPosition;

        if (enableIdleBreathing && !isHovered && !isPressed)
        {
            breathingTimer += Time.unscaledDeltaTime * breathingSpeed;
            float sineWave = Mathf.Sin(breathingTimer) * breathingScaleAmount;
            desiredScale = originalScale * (1f + sineWave);
        }

        // Smooth spring physics for scale
        rectTransform.localScale = Vector3.SmoothDamp(
            rectTransform.localScale,
            desiredScale,
            ref scaleVelocity,
            1f / springSpeed,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );

        // Smooth spring physics for 3D press depression
        rectTransform.anchoredPosition = Vector2.SmoothDamp(
            rectTransform.anchoredPosition,
            desiredPos,
            ref posVelocity,
            1f / springSpeed,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (!isPressed)
        {
            targetScale = originalScale * hoverScale;
            targetPosition = originalPosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isPressed)
        {
            targetScale = originalScale;
            targetPosition = originalPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = originalScale * pressedScale;
        targetPosition = originalPosition + new Vector2(0f, pressedOffsetY);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetScale = isHovered ? originalScale * hoverScale : originalScale;
        targetPosition = originalPosition;
    }
}
