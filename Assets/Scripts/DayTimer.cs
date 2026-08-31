using UnityEngine;
using TMPro;

public class DayTimer : MonoBehaviour
{
    [Header("ตั้งค่าเวลา")]
    public float dayDuration = 180f; // 3 นาที
    private float timeRemaining;
    private bool isDayEnded = false;

    [Header("UI")]
    public TextMeshProUGUI timerTextUI;
    public SummaryUI summaryUI;

    void Start()
    {
        timeRemaining = dayDuration;
    }

    void Update()
    {
        if (isDayEnded) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            EndDay();
        }
    }

    void UpdateTimerUI()
    {
        if (timerTextUI != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerTextUI.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    void EndDay()
    {
        isDayEnded = true;
        Debug.Log("☀️ หมดเวลากลางวันแล้ว! เปิดหน้าต่างสรุปผล...");

        if (summaryUI != null)
        {
            summaryUI.ShowSummaryPanel();
        }
    }
}