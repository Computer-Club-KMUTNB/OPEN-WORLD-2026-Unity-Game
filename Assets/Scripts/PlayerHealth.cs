using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI References")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Damage Flash Effect")]
    public Image damageFlashImage;
    [Range(0f, 1f)] public float maxFlashAlpha = 0.45f;
    public float fadeSpeed = 2.0f;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.25f;
    public float shakePosMagnitude = 0.15f;
    public float shakeRotMagnitude = 2.5f;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public string defaultRestaurantSceneName = "Dev_Restaurant_Flow";

    private bool isDead = false;
    private Texture2D blackTexture;

    private void Awake()
    {
        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.white);
        blackTexture.Apply();
    }

    void Start()
    {
        Time.timeScale = 1f;
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = 0f;
            damageFlashImage.color = c;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (damageFlashImage != null && damageFlashImage.color.a > 0f)
        {
            Color c = damageFlashImage.color;
            c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.deltaTime);
            damageFlashImage.color = c;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = maxFlashAlpha;
            damageFlashImage.color = c;
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakePosMagnitude, shakeRotMagnitude);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth:0} / {maxHealth:0}";
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("💀 Player defeated! Triggering Horrible Game Over screen.");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reset any temporary hunt meat count upon death
        InventoryManager.globalMeatCount = 0;
        InventoryManager.globalPorkCount = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Use custom horrific Death Screen
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToRestaurant()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        string target = defaultRestaurantSceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "restaurant-scene";
        }
        SceneManager.LoadScene(target);
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game!");
    }

    private void DrawBox(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, blackTexture);
        GUI.color = old;
    }

    private void OnGUI()
    {
        if (!isDead) return;

        // 1. Fullscreen Blood Vignette & Dread Background
        DrawBox(new Rect(0, 0, Screen.width, Screen.height), new Color(0.06f, 0.01f, 0.01f, 0.95f));

        // 2. Central Death Card
        float cardW = 680;
        float cardH = 430;
        float cardX = (Screen.width - cardW) / 2f;
        float cardY = (Screen.height - cardH) / 2f;

        DrawBox(new Rect(cardX, cardY, cardW, cardH), new Color(0.12f, 0.02f, 0.02f, 0.98f));

        // Red border accents
        DrawBox(new Rect(cardX, cardY, cardW, 4), new Color(0.85f, 0.15f, 0.15f, 1f));
        DrawBox(new Rect(cardX, cardY + cardH - 4, cardW, 4), new Color(0.85f, 0.15f, 0.15f, 1f));

        // 3. Menacing Header
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 42;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = new Color(0.92f, 0.15f, 0.15f);

        GUI.Label(new Rect(cardX, cardY + 24, cardW, 50), "YOU DIED", titleStyle);

        // 4. Subtitle / Dread Quote
        GUIStyle subTitleStyle = new GUIStyle(GUI.skin.label);
        subTitleStyle.fontSize = 15;
        subTitleStyle.fontStyle = FontStyle.Bold;
        subTitleStyle.alignment = TextAnchor.MiddleCenter;
        subTitleStyle.normal.textColor = new Color(1f, 0.45f, 0.45f);

        GUI.Label(new Rect(cardX, cardY + 80, cardW, 24), "SLAUGHTERED IN THE DUNGEON DEPTHS", subTitleStyle);

        // 5. Horrific Loss Details
        float infoBoxW = cardW - 60;
        float infoBoxH = 110;
        float infoBoxX = cardX + 30;
        float infoBoxY = cardY + 118;

        DrawBox(new Rect(infoBoxX, infoBoxY, infoBoxW, infoBoxH), new Color(0.04f, 0.01f, 0.01f, 0.9f));

        GUIStyle descStyle = new GUIStyle(GUI.skin.label);
        descStyle.fontSize = 13;
        descStyle.fontStyle = FontStyle.Normal;
        descStyle.alignment = TextAnchor.MiddleCenter;
        descStyle.wordWrap = true;
        descStyle.normal.textColor = new Color(0.9f, 0.85f, 0.85f);

        string loreText = "Your body was torn apart by relentless dungeon beasts.\n" +
                          "All harvested meats and valuable loot were lost to the abyss.\n" +
                          "Your restaurant's patrons will go hungry tonight.";
        GUI.Label(new Rect(infoBoxX + 16, infoBoxY + 14, infoBoxW - 32, 82), loreText, descStyle);

        // 6. Penalty summary line
        GUIStyle penaltyStyle = new GUIStyle(GUI.skin.label);
        penaltyStyle.fontSize = 12;
        penaltyStyle.fontStyle = FontStyle.Bold;
        penaltyStyle.alignment = TextAnchor.MiddleCenter;
        penaltyStyle.normal.textColor = new Color(0.95f, 0.35f, 0.35f);

        GUI.Label(new Rect(cardX, cardY + 242, cardW, 20), "PENALTY: All Hunted Meats Lost  •  0 Gold Earned", penaltyStyle);

        // 7. Action Buttons
        float btnW = 280;
        float btnH = 46;
        float btn1X = cardX + (cardW / 2f) - btnW - 14;
        float btn2X = cardX + (cardW / 2f) + 14;
        float btnY = cardY + 285;

        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 13;
        btnStyle.fontStyle = FontStyle.Bold;
        btnStyle.alignment = TextAnchor.MiddleCenter;
        btnStyle.normal.textColor = Color.white;

        if (GUI.Button(new Rect(btn1X, btnY, btnW, btnH), "Retry Expedition", btnStyle))
        {
            RestartGame();
        }

        if (GUI.Button(new Rect(btn2X, btnY, btnW, btnH), "Return to Restaurant", btnStyle))
        {
            ReturnToRestaurant();
        }

        // 8. Quit Button below
        float quitW = 180;
        float quitH = 34;
        float quitX = (Screen.width - quitW) / 2f;
        float quitY = btnY + btnH + 18;

        GUIStyle quitBtnStyle = new GUIStyle(GUI.skin.button);
        quitBtnStyle.fontSize = 11;
        quitBtnStyle.fontStyle = FontStyle.Normal;
        quitBtnStyle.alignment = TextAnchor.MiddleCenter;
        quitBtnStyle.normal.textColor = new Color(0.8f, 0.7f, 0.7f);

        if (GUI.Button(new Rect(quitX, quitY, quitW, quitH), "Give Up & Quit", quitBtnStyle))
        {
            QuitGame();
        }
    }
}