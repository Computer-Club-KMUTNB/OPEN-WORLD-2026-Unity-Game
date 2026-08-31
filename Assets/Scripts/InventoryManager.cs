using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    [Tooltip("ลาก MeatCounterText มาใส่ที่ช่องนี้")]
    public TextMeshProUGUI meatText;
    [Tooltip("ลาก PorkCounterText มาใส่ที่ช่องนี้ (ถ้ามี)")]
    public TextMeshProUGUI porkText;

    [Header("Loot Count")]
    public int meatCount = 0; // เนื้อวัว (RawBeef)
    public int porkCount = 0; // เนื้อหมู (RawPork)

    void Awake()
    {
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

    // ฟังก์ชันเพิ่มจำนวนเนื้อวัว
    public void AddMeat(int amount = 1)
    {
        meatCount += amount;
        UpdateMeatUI();
        Debug.Log("🥩 จำนวนเนื้อวัวปัจจุบัน: " + meatCount);
    }

    // ฟังก์ชันเพิ่มจำนวนเนื้อหมู
    public void AddPork(int amount = 1)
    {
        porkCount += amount;
        UpdateMeatUI();
        Debug.Log("🥓 จำนวนเนื้อหมูปัจจุบัน: " + porkCount);
    }

    // อัปเดตข้อความบนจอ
    public void UpdateMeatUI()
    {
        if (meatText != null)
        {
            if (porkCount > 0 && porkText == null)
            {
                meatText.text = $"Beef: {meatCount} | Pork: {porkCount}";
            }
            else
            {
                meatText.text = "Meat: " + meatCount;
            }
        }
        if (porkText != null)
        {
            porkText.text = "Pork: " + porkCount;
        }
    }
}