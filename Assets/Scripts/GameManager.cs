using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Static persistent backing fields to guarantee 100% data retention across all scenes
    public static int globalMoney = 0;
    public static int globalBeef = 0;
    public static int globalPork = 0;
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

    [Header("Summary Stats")]
    public int customersServedToday = 0;
    public int dishesCookedToday = 0;
    public int moneyEarnedToday = 0;
    public int moneySpentToday = 0;
    public int tipsEarnedToday = 0;

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

    // ฟังก์ชันเสิร์ฟอาหารและได้เงินแบบพื้นฐาน
    public void ServeFood()
    {
        RecordFoodServed(50, 0);
    }

    // ฟังก์ชันเสิร์ฟอาหาร (รับค่าราคา และทิป)
    public void RecordFoodServed(int price, int tip)
    {
        int totalReceived = price + tip;
        playerMoney += totalReceived;
        
        // บันทึกลงสถิติประจำวัน
        customersServedToday++;
        moneyEarnedToday += price;
        tipsEarnedToday += tip;

        UpdateMoneyText();
        SaveSystem.Save();
        Debug.Log($"🍽️ Food Served! Got money {price} + Tip {tip} Baht | Total Money: {playerMoney}");
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSystem.Save();
        }
    }

    // ฟังก์ชันบันทึกการทำอาหารเสร็จ
    public void RecordDishCooked()
    {
        dishesCookedToday++;
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
            moneySpentToday += amount;
            UpdateMoneyText();
            return true;
        }
        return false;
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
        SaveSystem.Save();
    }

    // คำนวณกำไรประจำวัน
    public int GetNetProfitToday()
    {
        return (moneyEarnedToday + tipsEarnedToday) - moneySpentToday;
    }

    // รีเซ็ตสถิติเพื่อเตรียมใช้ในวันถัดไป
    public void ResetDailyStats()
    {
        customersServedToday = 0;
        dishesCookedToday = 0;
        moneyEarnedToday = 0;
        moneySpentToday = 0;
        tipsEarnedToday = 0;
    }
}