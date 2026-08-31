using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public static int globalMeatCount = 0; // Raw Beef
    public static int globalPorkCount = 0; // Raw Pork

    [Header("UI References")]
    [Tooltip("ลาก MeatCounterText จากใต้ Canvas มาใส่")]
    public TextMeshProUGUI meatText;
    [Tooltip("ลาก PorkCounterText จากใต้ Canvas มาใส่")]
    public TextMeshProUGUI porkText;

    [Header("Loot Count")]
    public int meatCount
    {
        get => globalMeatCount;
        set
        {
            globalMeatCount = value;
            UpdateUI();
        }
    }

    public int porkCount
    {
        get => globalPorkCount;
        set
        {
            globalPorkCount = value;
            UpdateUI();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            if (meatText != null) Instance.meatText = meatText;
            if (porkText != null) Instance.porkText = porkText;
            Destroy(gameObject);
            return;
        }

        AutoFindUI();
    }

    void Start()
    {
        AutoFindUI();
        UpdateUI();
    }

    public void ResetInventory()
    {
        globalMeatCount = 0;
        globalPorkCount = 0;
        UpdateUI();
    }

    public void AutoFindUI()
    {
        if (meatText == null)
        {
            GameObject obj = GameObject.Find("MeatCounterText");
            if (obj == null) obj = GameObject.Find("MeatText");
            if (obj != null) meatText = obj.GetComponent<TextMeshProUGUI>();
        }

        if (porkText == null)
        {
            GameObject obj = GameObject.Find("PorkCounterText");
            if (obj == null) obj = GameObject.Find("PorkText");
            if (obj != null) porkText = obj.GetComponent<TextMeshProUGUI>();
        }
    }

    public void FindMeatUIIfMissing() => AutoFindUI();

    // ฟังก์ชันเพิ่มจำนวนเนื้อวัว
    public void AddMeat(int amount = 1)
    {
        globalMeatCount += amount;
        UpdateUI();
        Debug.Log($"🥩 ได้รับ Meat (Raw Beef) +{amount}! ยอดรวม: {globalMeatCount}");
    }

    // ฟังก์ชันเพิ่มจำนวนเนื้อหมู
    public void AddPork(int amount = 1)
    {
        globalPorkCount += amount;
        UpdateUI();
        Debug.Log($"🍖 ได้รับ Pork (Raw Pork) +{amount}! ยอดรวม: {globalPorkCount}");
    }

    // อัปเดตข้อความบนจอ
    public void UpdateMeatUI() => UpdateUI();

    public void UpdateUI()
    {
        AutoFindUI();
        if (meatText != null && meatText.gameObject.activeSelf)
        {
            meatText.gameObject.SetActive(false);
        }
        if (porkText != null && porkText.gameObject.activeSelf)
        {
            porkText.gameObject.SetActive(false);
        }
    }
}