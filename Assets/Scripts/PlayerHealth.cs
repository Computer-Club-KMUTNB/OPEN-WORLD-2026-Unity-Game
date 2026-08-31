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
    [Tooltip("ระยะเวลาที่กล้องสั่น (วินาที)")]
    public float shakeDuration = 0.25f;
    [Tooltip("ความแรงในการขยับตำแหน่งกล้อง")]
    public float shakePosMagnitude = 0.15f;
    [Tooltip("ความแรงในการบิดเอียงมุมกล้อง")]
    public float shakeRotMagnitude = 2.5f;

    [Header("Game Over UI")]
    [Tooltip("ลาก GameOverPanel มาใส่ช่องนี้")]
    public GameObject gameOverPanel;

    private bool isDead = false;

    void Start()
    {
        // คืนค่าเวลาปกติทุกครั้งที่เริ่มเกม หรือโหลดฉากใหม่
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
            gameOverPanel.SetActive(false); // ซ่อนหน้าต่าง Game Over ไว้ก่อนเริ่มเล่น
        }
    }

    void Update()
    {
        // ค่อยๆ ลดความเข้มของสีแดงบนหน้าจอ
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

        // 1. เรียกเอฟเฟกต์หน้าจอแดง
        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = maxFlashAlpha;
            damageFlashImage.color = c;
        }

        // 2. เรียกกล้องสั่น (ส่งค่าแรงสั่นและมุมเอียง)
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
        Debug.Log("💀 ผู้เล่นตายแล้ว (Game Over)");

        // ปลดล็อกเคอร์เซอร์เมาส์ให้คลิกปุ่มบนหน้าจอได้
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // แสดงหน้าต่าง Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // หยุดเวลาในเกม
        Time.timeScale = 0f;
    }

    // ฟังก์ชันสำหรับปุ่ม Restart (โหลดฉากปัจจุบันใหม่)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ฟังก์ชันสำหรับปุ่ม Return to Restaurant / Main Menu
    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // ฟังก์ชันสำหรับปุ่ม Quit
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("ออกจากเกม!");
    }
}