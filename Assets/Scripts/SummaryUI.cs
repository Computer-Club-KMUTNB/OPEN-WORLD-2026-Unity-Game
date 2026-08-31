using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SummaryUI : MonoBehaviour
{
    [Header("ข้อความแสดงผล UI")]
    public TextMeshProUGUI customersServedText;
    public TextMeshProUGUI dishesCookedText;
    public TextMeshProUGUI moneyEarnedText;
    public TextMeshProUGUI moneySpentText;
    public TextMeshProUGUI tipsEarnedText;
    public TextMeshProUGUI netProfitText;

    [Header("ชื่อฉากล่าสัตว์")]
    public string huntingSceneName = "HuntingScene";

    // ฟังก์ชันแสดงหน้าต่างสรุปผลและอัปเดตตัวเลข
    public void ShowSummaryPanel()
    {
        gameObject.SetActive(true); // เปิดหน้าต่างสรุปผล

        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            if (customersServedText != null) customersServedText.text = "ลูกค้าที่เสิร์ฟ: " + gm.customersServedToday + " คน";
            if (dishesCookedText != null) dishesCookedText.text = "อาหารที่ทำ: " + gm.dishesCookedToday + " จาน";
            if (moneyEarnedText != null) moneyEarnedText.text = "รายได้ขายอาหาร: +" + gm.moneyEarnedToday + " บาท";
            if (moneySpentText != null) moneySpentText.text = "ค่าวัตถุดิบที่จ่าย: -" + gm.moneySpentToday + " บาท";
            if (tipsEarnedText != null) tipsEarnedText.text = "ทิปที่ได้รับ: +" + gm.tipsEarnedToday + " บาท";

            int netProfit = gm.GetNetProfitToday();
            if (netProfitText != null)
            {
                netProfitText.text = "กำไรสุทธิวันนี้: " + (netProfit >= 0 ? "+" : "") + netProfit + " บาท";
                netProfitText.color = netProfit >= 0 ? Color.green : Color.red;
            }
        }

        // ปลดล็อกเมาส์ให้กดปุ่มบนหน้าจอได้
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ฟังก์ชันเมื่อกดปุ่ม "ออกไปล่าสัตว์ (เข้าสู่ช่วงกลางคืน)"
    public void OnClickGoHunting()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.ResetDailyStats(); // ล้างสถิติของวันนี้เพื่อเตรียมไว้วันถัดไป
        }

        // โหลดเข้าสู่ฉากล่าสัตว์
        SceneManager.LoadScene(huntingSceneName);
    }
}