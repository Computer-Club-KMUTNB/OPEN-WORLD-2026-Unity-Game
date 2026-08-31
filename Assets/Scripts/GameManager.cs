using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Static persistent backing fields to guarantee 100% data retention across all scenes
    public static int globalMoney = 250;
    public static int globalBeef = 5;
    public static int globalPork = 5;
    public static int globalRice = 5;
    public static int globalVeggie = 5;

    [Header("ระบบเงิน")]
    public TextMeshProUGUI moneyTextUI; 

    public int playerMoney
    {
        get => globalMoney;
        set
        {
            globalMoney = value;
            UpdateMoneyText();
        }
    }

    [Header("ระบบสต็อควัตถุดิบ")]
    public int rawBeefStock
    {
        get => globalBeef;
        set => globalBeef = Mathf.Max(0, value);
    }

    public int rawPorkStock
    {
        get => globalPork;
        set => globalPork = Mathf.Max(0, value);
    }

    public int rawRiceStock
    {
        get => globalRice;
        set => globalRice = Mathf.Max(0, value);
    }

    public int rawVeggieStock
    {
        get => globalVeggie;
        set => globalVeggie = Mathf.Max(0, value);
    }

    void Awake()
    {
        SaveSystem.InitializeOnStartup();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // If another instance exists, copy over moneyTextUI reference before destroying
            if (moneyTextUI != null)
            {
                Instance.moneyTextUI = moneyTextUI;
            }
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndBindMoneyUI();
        UpdateMoneyText();
    }

    void Start()
    {
        FindAndBindMoneyUI();
        UpdateMoneyText();
    }

    public void FindAndBindMoneyUI()
    {
        if (moneyTextUI == null)
        {
            GameObject moneyObj = GameObject.Find("MoneyText");
            if (moneyObj == null) moneyObj = GameObject.Find("MoneyTextUI");
            if (moneyObj != null)
            {
                moneyTextUI = moneyObj.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    // ฟังก์ชันเสิร์ฟอาหารและได้เงิน
    public void ServeFood()
    {
        playerMoney += 50; 
        UpdateMoneyText(); 
        Debug.Log($"🍽️ Food served! Money: {playerMoney} Baht");
    }

    public void UpdateMoneyText()
    {
        if (moneyTextUI != null)
        {
            moneyTextUI.text = "Money: " + playerMoney;
        }
    }

    // สำหรับเช็คและหักเงิน (รีเทิร์น true ถ้าเงินพอและหักสำเร็จ)
    public bool DeductMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            UpdateMoneyText();
            return true;
        }
        return false; // เงินไม่พอ
    }

    // สำหรับรับเนื้อสัตว์จากการล่า
    public void AddHuntingLoot(string meatType, int amount)
    {
        if (meatType == "RawBeef")
        {
            rawBeefStock += amount;
            Debug.Log($"🥩 ได้รับเนื้อวัวจากการล่ามาเพิ่ม +{amount}! สต็อคปัจจุบัน: {rawBeefStock}");
        }
        else if (meatType == "RawPork")
        {
            rawPorkStock += amount;
            Debug.Log($"🥓 ได้รับเนื้อหมูจากการล่ามาเพิ่ม +{amount}! สต็อคปัจจุบัน: {rawPorkStock}");
        }
    }
}