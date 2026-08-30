using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    [Tooltip("ลาก MeatCounterText มาใส่ที่ช่องนี้")]
    public TextMeshProUGUI meatText;

    private int meatCount = 0;

    void Awake()
    {
        // ทำ Singleton เพื่อให้สคริปต์อื่นเรียกใช้ได้ทันที
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateMeatUI();
    }

    // ฟังก์ชันเพิ่มจำนวนเนื้อ
    public void AddMeat(int amount = 1)
    {
        meatCount += amount;
        UpdateMeatUI();
        Debug.Log("🥩 จำนวนเนื้อปัจจุบัน: " + meatCount);
    }

    // อัปเดตข้อความบนจอ
    void UpdateMeatUI()
    {
        if (meatText != null)
        {
            meatText.text = "Meat: " + meatCount;
        }
    }
}