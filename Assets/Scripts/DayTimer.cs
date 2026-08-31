using UnityEngine;
using TMPro;

public class DayTimer : MonoBehaviour
{
    [Header("ตั้งค่าเวลา")]
    public float dayDuration = 180f; // เวลาใน 1 วัน (เช่น 180 วินาที = 3 นาที)
    public float timeRemaining;
    public bool isShopOpen = false;  // สถานะเปิด/ปิดร้าน
    public bool isDayEnded = false;

    [Header("UI & Systems")]
    public TextMeshProUGUI timerTextUI;

    [Header("Customer Spawner")]
    [Tooltip("ลากวัตถุ CustomerSpawner จาก Hierarchy มาใส่")]
    public GameObject customerSpawnerObj; 

    void Start()
    {
        timeRemaining = dayDuration;
        UpdateTimerUI();

        // เริ่มเกมมา ปิดจุดเสกลูกค้าให้ซ่อนไว้ก่อน (รอจนกว่าจะกดกระดิ่งหรือเริ่มกะ)
        if (customerSpawnerObj != null) customerSpawnerObj.SetActive(false);
    }

    void Update()
    {
        // ถ้าร้านยังไม่เปิด หรือ หมดวันแล้ว ➔ ไม่ต้องนับเวลา
        if (!isShopOpen || isDayEnded) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            CloseShop(); // หมดเวลา สั่งปิดร้าน
        }
    }

    public void OpenShop()
    {
        if (isShopOpen) return;

        isShopOpen = true;
        isDayEnded = false;
        Debug.Log("🔔 เปิดร้านแล้ว! เริ่มนับเวลาและเรียกลูกค้า");

        // เปิดจุดเสกลูกค้าให้ทำงาน
        if (customerSpawnerObj != null) customerSpawnerObj.SetActive(true);

        CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
        if (spawner != null) spawner.StartShift();
    }

    public void CloseShop()
    {
        if (isDayEnded) return;

        isShopOpen = false;
        isDayEnded = true;
        Debug.Log("🔔 ปิดร้านแล้ว! หยุดรับลูกค้าและเปิดหน้าต่างสรุปผล");

        // ปิดจุดเสกลูกค้า ไม่ให้คนเข้ามาเพิ่ม
        if (customerSpawnerObj != null) customerSpawnerObj.SetActive(false);

        CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
        if (spawner != null) spawner.StopShift();

        EndDay();
    }

    public void EndDay()
    {
        isDayEnded = true;
        Debug.Log("☀️ หมดเวลากลางวันแล้ว! เปิดหน้าต่างสรุปผล...");

        RestaurantFlowController flow = FindFirstObjectByType<RestaurantFlowController>();
        if (flow != null)
        {
            flow.EndRestaurantShift();
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
}