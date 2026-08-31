using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ตัวแปร Singleton เพื่อให้สคริปต์อื่น (เช่น คอมพิวเตอร์ หรือ กล่องสต็อค) เรียกใช้งานได้ง่าย
    public static GameManager Instance;

    [Header("ระบบเงิน")]
    public int playerMoney = 0;
    public TextMeshProUGUI moneyTextUI; 

    [Header("ระบบสต็อควัตถุดิบ")]
    public int rawBeefStock = 5;   // ได้จากการล่าสัตว์ (ข้ามมาจากอีกฉาก)
    public int rawPorkStock = 5;   // ได้จากการล่าสัตว์ (ข้ามมาจากอีกฉาก)
    public int rawRiceStock = 5;   // สั่งจากคอมพิวเตอร์
    public int rawVeggieStock = 5;  // สั่งจากคอมพิวเตอร์

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

    // ฟังก์ชันเสิร์ฟอาหารและได้เงิน
    public void ServeFood()
    {
        playerMoney += 50; 
        
        // พอได้เงินสั่งให้อัปเดต UI
        UpdateMoneyText(); 
        
        Debug.Log("Food served! Money: " + playerMoney + " Baht");
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
            UpdateMoneyText();
            return true;
        }
        return false; // เงินไม่พอ
    }

    // สำหรับรับเนื้อสัตว์
    public void AddHuntingLoot(string meatType, int amount)
    {
        if (meatType == "RawBeef")
        {
            rawBeefStock += amount;
            Debug.Log("ได้รับเนื้อวัวจากการล่ามาเพิ่ม! สต็อคปัจจุบัน: " + rawBeefStock);
        }
        else if (meatType == "RawPork")
        {
            rawPorkStock += amount;
            Debug.Log("ได้รับเนื้อหมูจากการล่ามาเพิ่ม! สต็อคปัจจุบัน: " + rawPorkStock);
        }
    }
}