using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HuntAtmosphereManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite emberSprite;

    [Header("Ember Settings")]
    [SerializeField] private int emberCount = 28;
    [SerializeField] private float minEmberSpeed = 35f;
    [SerializeField] private float maxEmberSpeed = 85f;
    [SerializeField] private float minEmberSize = 12f;
    [SerializeField] private float maxEmberSize = 38f;

    [Header("Vignette / Pulse")]
    [SerializeField] private Image pulseVignette;
    [SerializeField] private float pulseSpeed = 1.2f;

    private List<EmberParticle> embers = new List<EmberParticle>();
    private RectTransform canvasRect;

    private class EmberParticle
    {
        public RectTransform rect;
        public Image img;
        public float speed;
        public float swaySpeed;
        public float swayAmount;
        public float phase;
        public float alpha;
        public float startX;
    }

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        SpawnEmbers();
    }

    private void SpawnEmbers()
    {
        if (emberSprite == null || canvasRect == null) return;

        float width = canvasRect.rect.width > 0 ? canvasRect.rect.width : 1920f;
        float height = canvasRect.rect.height > 0 ? canvasRect.rect.height : 1080f;

        for (int i = 0; i < emberCount; i++)
        {
            GameObject obj = new GameObject("Ember_" + i, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(transform, false);

            RectTransform rt = obj.GetComponent<RectTransform>();
            Image img = obj.GetComponent<Image>();

            img.sprite = emberSprite;
            img.raycastTarget = false;

            float size = Random.Range(minEmberSize, maxEmberSize);
            rt.sizeDelta = new Vector2(size, size);

            float x = Random.Range(-width * 0.5f, width * 0.5f);
            float y = Random.Range(-height * 0.5f, height * 0.5f);
            rt.anchoredPosition = new Vector2(x, y);

            // Fiery orange/yellow/crimson color variation
            float colorT = Random.value;
            Color emberColor = Color.Lerp(new Color(1f, 0.2f, 0.05f, 0.9f), new Color(1f, 0.85f, 0.2f, 0.95f), colorT);
            img.color = emberColor;

            EmberParticle ep = new EmberParticle
            {
                rect = rt,
                img = img,
                speed = Random.Range(minEmberSpeed, maxEmberSpeed),
                swaySpeed = Random.Range(1.0f, 2.5f),
                swayAmount = Random.Range(15f, 40f),
                phase = Random.Range(0f, Mathf.PI * 2f),
                alpha = Random.Range(0.6f, 0.95f),
                startX = x
            };

            embers.Add(ep);
        }
    }

    private void Update()
    {
        if (canvasRect == null) return;

        float height = canvasRect.rect.height > 0 ? canvasRect.rect.height : 1080f;
        float halfH = height * 0.5f;
        float dt = Time.unscaledDeltaTime; // Unscaled so embers float even when paused!

        // Update Embers
        for (int i = 0; i < embers.Count; i++)
        {
            EmberParticle ep = embers[i];
            Vector2 pos = ep.rect.anchoredPosition;

            pos.y += ep.speed * dt;
            pos.x = ep.startX + Mathf.Sin(Time.unscaledTime * ep.swaySpeed + ep.phase) * ep.swayAmount;

            // Wrap to bottom when reaching top
            if (pos.y > halfH + 40f)
            {
                pos.y = -halfH - 40f;
                float width = canvasRect.rect.width > 0 ? canvasRect.rect.width : 1920f;
                ep.startX = Random.Range(-width * 0.5f, width * 0.5f);
                pos.x = ep.startX;
            }

            ep.rect.anchoredPosition = pos;

            // Twinkle / flame flicker
            float flicker = Mathf.PingPong(Time.unscaledTime * 2.5f + ep.phase, 0.35f);
            Color c = ep.img.color;
            c.a = ep.alpha - flicker;
            ep.img.color = c;
        }

        // Pulse Vignette
        if (pulseVignette != null)
        {
            float pulse = 0.2f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.08f;
            Color vc = pulseVignette.color;
            vc.a = pulse;
            pulseVignette.color = vc;
        }
    }
}
