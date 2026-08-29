using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FloatingElement : MonoBehaviour
{
    [Header("Floating Movement")]
    [Tooltip("Vertical bobbing amplitude in pixels")]
    [SerializeField] private float floatAmplitudeY = 10f;
    [Tooltip("Horizontal swaying amplitude in pixels")]
    [SerializeField] private float floatAmplitudeX = 4f;
    [Tooltip("Floating speed frequency")]
    [SerializeField] private float floatFrequency = 1.8f;

    [Header("Gentle Rotation / Tilting")]
    [SerializeField] private bool enableTilt = true;
    [SerializeField] private float tiltAngle = 2.5f;
    [SerializeField] private float tiltFrequency = 1.2f;

    [Header("Randomization")]
    [Tooltip("Randomize phase so multiple elements don't move in sync")]
    [SerializeField] private bool randomizePhase = true;

    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;
    private float phaseOffsetX;
    private float phaseOffsetY;
    private float phaseOffsetTilt;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalAnchoredPosition = rectTransform.anchoredPosition;

        if (randomizePhase)
        {
            phaseOffsetX = Random.Range(0f, Mathf.PI * 2f);
            phaseOffsetY = Random.Range(0f, Mathf.PI * 2f);
            phaseOffsetTilt = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    private void OnEnable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }
    }

    private void Update()
    {
        float time = Time.unscaledTime;

        // Calculate floating offsets
        float offsetY = Mathf.Sin((time * floatFrequency) + phaseOffsetY) * floatAmplitudeY;
        float offsetX = Mathf.Cos((time * floatFrequency * 0.7f) + phaseOffsetX) * floatAmplitudeX;

        rectTransform.anchoredPosition = originalAnchoredPosition + new Vector2(offsetX, offsetY);

        // Optional gentle tilt
        if (enableTilt)
        {
            float angle = Mathf.Sin((time * tiltFrequency) + phaseOffsetTilt) * tiltAngle;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        }
    }
}
