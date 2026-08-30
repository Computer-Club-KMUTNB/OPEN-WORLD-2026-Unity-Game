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

        // หยุดเวลาในเกม (ศัตรูจะหยุดนิ่งทันที)
        Time.timeScale = 0f;
    }

    // ฟังก์ชันสำหรับปุ่ม Restart (โหลดฉากปัจจุบันใหม่)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ฟังก์ชันสำหรับปุ่ม Quit
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("ออกจากเกม!");
    }
}