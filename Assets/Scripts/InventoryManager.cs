using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    [Tooltip("ลาก MeatCounterText จากใต้ Canvas มาใส่")]
    public TextMeshProUGUI meatText;
    [Tooltip("ลาก PorkCounterText จากใต้ Canvas มาใส่")]
    public TextMeshProUGUI porkText;

    [Header("Current Inventory")]
    public int meatCount = 0;
    public int porkCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
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

    private void AutoFindUI()
    {
        if (meatText == null)
        {
            meatText = GameObject.Find("MeatCounterText")?.GetComponent<TextMeshProUGUI>();
            if (meatText == null)
            {
                meatText = GameObject.Find("MeatText")?.GetComponent<TextMeshProUGUI>();
            }
        }

        if (porkText == null)
        {
            porkText = GameObject.Find("PorkCounterText")?.GetComponent<TextMeshProUGUI>();
            if (porkText == null)
            {
                porkText = GameObject.Find("PorkText")?.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public void AddMeat(int amount = 1)
    {
        meatCount += amount;
        UpdateUI();
        Debug.Log($"🥩 ได้รับ Meat! ยอดรวม: {meatCount}");
    }

    public void AddPork(int amount = 1)
    {
        porkCount += amount;
        UpdateUI();
        Debug.Log($"🍖 ได้รับ Pork! ยอดรวม: {porkCount}");
    }

    public void UpdateUI()
    {
        if (meatText != null)
        {
            meatText.text = $"Meat : {meatCount}";
        }

        if (porkText != null)
        {
            porkText.text = $"Pork : {porkCount}";
        }
    }
}