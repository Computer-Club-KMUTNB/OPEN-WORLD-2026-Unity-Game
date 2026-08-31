using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuteAtmosphereManager : MonoBehaviour
{
    [Header("Sprite Assets")]
    [SerializeField] private Sprite bubbleSprite;
    [SerializeField] private Sprite sparkleStarSprite;
    [SerializeField] private Sprite heartSprite;

    [Header("Bubble Particle Settings")]
    [Range(5, 35)]
    [SerializeField] private int bubbleCount = 18;
    [SerializeField] private float minBubbleSpeed = 25f;
    [SerializeField] private float maxBubbleSpeed = 55f;
    [SerializeField] private float minBubbleSize = 35f;
    [SerializeField] private float maxBubbleSize = 95f;
    [SerializeField] private float minBubbleAlpha = 0.55f;
    [SerializeField] private float maxBubbleAlpha = 0.9f;

    [Header("Sparkle Star Settings")]
    [Range(5, 25)]
    [SerializeField] private int sparkleCount = 12;
    [SerializeField] private float minSparkleSize = 20f;
    [SerializeField] private float maxSparkleSize = 48f;

    [Header("Floating Heart Settings")]
    [Range(2, 15)]
    [SerializeField] private int heartCount = 6;

    private class FloatingParticle
    {
        public RectTransform rectTransform;
        public Image image;
        public Vector2 position;
        public float speed;
        public float wobbleSpeed;
        public float wobbleAmount;
        public float wobbleTimer;
        public float baseSize;
        public float rotationSpeed;
        public float currentRotation;
        public float baseAlpha;
        public float pulseSpeed;
        public float pulseTimer;
        public int particleType; // 0 = bubble, 1 = star, 2 = heart
    }

    private List<FloatingParticle> particles = new List<FloatingParticle>();
    private RectTransform canvasRect;
    private float screenWidth = 1920f;
    private float screenHeight = 1080f;

    private void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.rootCanvas != null)
        {
            canvasRect = canvas.rootCanvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                screenWidth = canvasRect.rect.width;
                screenHeight = canvasRect.rect.height;
            }
        }
    }

    private void Start()
    {
        InitializeParticles();
    }

    private void InitializeParticles()
    {
        // 1. Create Bubbles
        for (int i = 0; i < bubbleCount; i++)
        {
            CreateParticle(0, bubbleSprite, true);
        }

        // 2. Create Sparkle Stars
        for (int i = 0; i < sparkleCount; i++)
        {
            CreateParticle(1, sparkleStarSprite, true);
        }

        // 3. Create Floating Hearts
        for (int i = 0; i < heartCount; i++)
        {
            CreateParticle(2, heartSprite, true);
        }
    }

    private void CreateParticle(int type, Sprite sprite, bool randomInitialY)
    {
        if (sprite == null) return;

        GameObject go = new GameObject(type == 0 ? "Bubble" : (type == 1 ? "Sparkle" : "Heart"));
        go.transform.SetParent(transform, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        CanvasRenderer cr = go.AddComponent<CanvasRenderer>();
        cr.cullTransparentMesh = true;

        Image img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = false;
        img.maskable = false;

        FloatingParticle p = new FloatingParticle();
        p.rectTransform = rt;
        p.image = img;
        p.particleType = type;

        ResetParticle(p, randomInitialY);
        particles.Add(p);
    }

    private void ResetParticle(FloatingParticle p, bool randomY)
    {
        float halfW = screenWidth * 0.5f + 80f;
        float halfH = screenHeight * 0.5f + 80f;

        float posX = Random.Range(-halfW, halfW);
        float posY = randomY ? Random.Range(-halfH, halfH) : -halfH;

        p.position = new Vector2(posX, posY);
        p.wobbleTimer = Random.Range(0f, Mathf.PI * 2f);
        p.pulseTimer = Random.Range(0f, Mathf.PI * 2f);

        if (p.particleType == 0) // Bubble
        {
            p.speed = Random.Range(minBubbleSpeed, maxBubbleSpeed);
            p.wobbleSpeed = Random.Range(1.2f, 2.8f);
            p.wobbleAmount = Random.Range(18f, 45f);
            p.baseSize = Random.Range(minBubbleSize, maxBubbleSize);
            p.baseAlpha = Random.Range(minBubbleAlpha, maxBubbleAlpha);
            p.pulseSpeed = Random.Range(1.0f, 2.0f);
            p.rotationSpeed = Random.Range(-15f, 15f);
            p.image.color = new Color(1f, 1f, 1f, p.baseAlpha);
        }
        else if (p.particleType == 1) // Sparkle Star
        {
            p.speed = Random.Range(10f, 25f);
            p.wobbleSpeed = Random.Range(0.8f, 1.8f);
            p.wobbleAmount = Random.Range(10f, 25f);
            p.baseSize = Random.Range(minSparkleSize, maxSparkleSize);
            p.baseAlpha = Random.Range(0.4f, 0.95f);
            p.pulseSpeed = Random.Range(2.5f, 5.5f);
            p.rotationSpeed = Random.Range(20f, 60f) * (Random.value > 0.5f ? 1f : -1f);
            
            // Soft pastel tint for stars (gold, warm peach, cyan)
            float tintChoice = Random.value;
            if (tintChoice < 0.45f)
                p.image.color = new Color(1f, 0.95f, 0.75f, p.baseAlpha);
            else if (tintChoice < 0.75f)
                p.image.color = new Color(1f, 0.85f, 0.9f, p.baseAlpha);
            else
                p.image.color = new Color(0.85f, 0.95f, 1f, p.baseAlpha);
        }
        else // Heart
        {
            p.speed = Random.Range(15f, 35f);
            p.wobbleSpeed = Random.Range(1.5f, 3.0f);
            p.wobbleAmount = Random.Range(20f, 40f);
            p.baseSize = Random.Range(22f, 42f);
            p.baseAlpha = Random.Range(0.45f, 0.75f);
            p.pulseSpeed = Random.Range(1.8f, 3.2f);
            p.rotationSpeed = Random.Range(-25f, 25f);
            p.image.color = new Color(1f, 0.75f, 0.85f, p.baseAlpha);
        }

        p.rectTransform.sizeDelta = new Vector2(p.baseSize, p.baseSize);
        p.rectTransform.anchoredPosition = p.position;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        float halfH = screenHeight * 0.5f + 100f;

        for (int i = 0; i < particles.Count; i++)
        {
            FloatingParticle p = particles[i];

            // Move upward
            p.position.y += p.speed * dt;
            p.wobbleTimer += dt * p.wobbleSpeed;
            p.pulseTimer += dt * p.pulseSpeed;
            p.currentRotation += p.rotationSpeed * dt;

            // Wobble on X axis
            float wobbleX = Mathf.Sin(p.wobbleTimer) * p.wobbleAmount;
            Vector2 currentPos = new Vector2(p.position.x + wobbleX, p.position.y);
            p.rectTransform.anchoredPosition = currentPos;

            // Subtle pulsing & rotation
            float pulseScale = 1f + Mathf.Sin(p.pulseTimer) * 0.12f;
            p.rectTransform.localScale = new Vector3(pulseScale, pulseScale, 1f);
            p.rectTransform.localEulerAngles = new Vector3(0f, 0f, p.currentRotation);

            // Shimmer alpha for sparkle stars
            if (p.particleType == 1)
            {
                float shimmerAlpha = Mathf.Clamp01(p.baseAlpha * (0.55f + 0.45f * Mathf.Sin(p.pulseTimer * 1.5f)));
                Color c = p.image.color;
                c.a = shimmerAlpha;
                p.image.color = c;
            }

            // Respawn if drifted past top of screen
            if (p.position.y > halfH)
            {
                ResetParticle(p, false);
            }
        }
    }
}
