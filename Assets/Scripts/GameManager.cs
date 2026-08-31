using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("ระบบเงิน")]
    public int playerMoney = 0;
    public TextMeshProUGUI moneyTextUI; 

    [Header("ระบบสต็อควัตถุดิบ")]
    public int rawBeefStock = 5;   // ได้จากการล่าสัตว์ (ข้ามมาจากอีกฉาก)
    public int rawPorkStock = 5;   // ได้จากการล่าสัตว์ (ข้ามมาจากอีกฉาก)
    public int rawRiceStock = 5;   // สั่งจากคอมพิวเตอร์
    public int rawVeggieStock = 5;  // สั่งจากคอมพิวเตอร์

    [Header("Summary Stats")]
    public int customersServedToday = 0;
    public int dishesCookedToday = 0;
    public int moneyEarnedToday = 0;
    public int moneySpentToday = 0;
    public int tipsEarnedToday = 0;

    void Awake()
    {
        // ป้องกันไม่ให้มี GameManager ซ้ำซ้อน และเก็บข้อมูลไว้ไม่ให้หายเมื่อเปลี่ยนฉาก
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // อัปเดตข้อความเงินบนจอตั้งแต่เริ่มเกม
        UpdateMoneyText();
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
        Debug.Log($"Food Serve! Got money {price} + Tip {tip} Baht");
    }

    // ฟังก์ชันบันทึกการทำอาหารเสร็จ
    public void RecordDishCooked()
    {
        dishesCookedToday++;
    }

    // เปลี่ยนจาก void เป็น public void
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