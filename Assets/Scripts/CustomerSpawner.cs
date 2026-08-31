using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("ตั้งค่าการเรียกลูกค้า")]
    public GameObject customerPrefab; // ลูกค้าที่จะเสก
    public Transform spawnPoint;      // จุดที่จะให้ลูกค้าโผล่
    public float spawnInterval = 5f;  // เวลาหน่วงระหว่างเรียกลูกค้าแต่ละคน (วินาที)
    
    [Header("Shift Control (การควบคุมกะ)")]
    public bool autoStart = false;    // เริ่มกะทันทีที่โหลดฉากหรือไม่
    public bool isShiftActive = false; // สถานะกะปัจจุบัน

    void Start()
    {
        try
        {
            if (spawnPoint == null)
            {
                GameObject sp = GameObject.Find("SpawnPoint");
                if (sp == null) sp = GameObject.Find("CustomerSpawnPoint");
                if (sp != null) spawnPoint = sp.transform;
            }
        }
        catch (System.Exception)
        {
            spawnPoint = null;
        }

        if (autoStart)
        {
            isShiftActive = true;
        }

        // เริ่มต้นการเสกลูกค้าแบบวนลูป
        StartCoroutine(SpawnCustomerRoutine());
    }

    public void StartShift()
    {
        isShiftActive = true;
        Debug.Log("🍽️ Shift started! Customers will now arrive.");
    }

    public void StopShift()
    {
        isShiftActive = false;
        Debug.Log("🍽️ Shift stopped! No new customers will arrive.");
    }

    IEnumerator SpawnCustomerRoutine()
    {
        // ลูปนี้จะทำงานไปเรื่อยๆ ตลอดการเล่นเกม
        while (true)
        {
            // ถ้ายังไม่เริ่มกะ ให้รอ
            if (!isShiftActive)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // 1. เช็คก่อนว่ามีโต๊ะว่างเหลือ
            Seat[] allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
            int occupiedCount = 0;
            
            foreach (Seat seat in allSeats)
            {
                if (seat != null && seat.isOccupied) occupiedCount++;
            }

            // 2. ถ้าเก้าอี้ยังไม่เต็มร้านถึงจะเสกลูกค้าใหม่
            if (allSeats != null && allSeats.Length > 0 && occupiedCount < allSeats.Length && customerPrefab != null)
            {
                Vector3 pos = transform.position;
                Quaternion rot = transform.rotation;
                try
                {
                    if (spawnPoint != null)
                    {
                        pos = spawnPoint.position;
                        rot = spawnPoint.rotation;
                    }
                }
                catch (System.Exception)
                {
                    spawnPoint = null;
                    pos = transform.position;
                    rot = transform.rotation;
                }

                Instantiate(customerPrefab, pos, rot);
                Debug.Log("เสกลูกค้าใหม่เข้าร้านแล้ว!");
            }

            // 3. รอเวลา X วินาทีก่อนจะเช็คและเสกคนต่อไป
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}