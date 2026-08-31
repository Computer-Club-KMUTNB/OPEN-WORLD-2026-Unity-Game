using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public static int globalMeatCount = 0; // Raw Beef
    public static int globalPorkCount = 0; // Raw Pork

    [Header("UI References")]
    [Tooltip("ลาก MeatCounterText มาใส่ที่ช่องนี้")]
    public TextMeshProUGUI meatText;
    [Tooltip("ลาก PorkCounterText มาใส่ที่ช่องนี้ (ถ้ามี)")]
    public TextMeshProUGUI porkText;

    [Header("Loot Count")]
    public int meatCount
    {
        get => globalMeatCount;
        set
        {
            globalMeatCount = value;
            UpdateMeatUI();
        }
    }

    public int porkCount
    {
        get => globalPorkCount;
        set
        {
            globalPorkCount = value;
            UpdateMeatUI();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            if (meatText != null) Instance.meatText = meatText;
            if (porkText != null) Instance.porkText = porkText;
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindMeatUIIfMissing();
        UpdateMeatUI();
    }

    public void ResetInventory()
    {
        globalMeatCount = 0;
        globalPorkCount = 0;
        UpdateMeatUI();
    }

    private void FindMeatUIIfMissing()
    {
        if (meatText == null)
        {
            GameObject obj = GameObject.Find("MeatCounterText");
            if (obj == null) obj = GameObject.Find("MeatText");
            if (obj != null) meatText = obj.GetComponent<TextMeshProUGUI>();
        }
    }

    // ฟังก์ชันเพิ่มจำนวนเนื้อวัว
    public void AddMeat(int amount = 1)
    {
        globalMeatCount += amount;
        UpdateMeatUI();
        Debug.Log($"🥩 Raw Beef collected +{amount}! Total: {globalMeatCount}");
    }

    // ฟังก์ชันเพิ่มจำนวนเนื้อหมู
    public void AddPork(int amount = 1)
    {
        globalPorkCount += amount;
        UpdateMeatUI();
        Debug.Log($"🥓 Raw Pork collected +{amount}! Total: {globalPorkCount}");
    }

    // อัปเดตข้อความบนจอ
    public void UpdateMeatUI()
    {
        FindMeatUIIfMissing();
        if (meatText != null)
        {
            if (globalPorkCount > 0 && porkText == null)
            {
                meatText.text = $"Beef: {globalMeatCount} | Pork: {globalPorkCount}";
            }
            else
            {
                meatText.text = $"Meat: {globalMeatCount}";
            }
        }
        if (porkText != null)
        {
            porkText.text = $"Pork: {globalPorkCount}";
        }
    }
}