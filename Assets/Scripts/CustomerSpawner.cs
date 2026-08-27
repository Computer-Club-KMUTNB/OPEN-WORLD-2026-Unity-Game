using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("ตั้งค่าการเรียกลูกค้า")]
    public GameObject customerPrefab; // ลูกค้าที่จะเสก
    public Transform spawnPoint;      // จุดที่จะให้ลูกค้าโผล่
    public float spawnInterval = 5f;  // เวลาหน่วงระหว่างเรียกลูกค้าแต่ละคน (วินาที)

    void Start()
    {
        // เริ่มต้นการเสกลูกค้าแบบวนลูป
        StartCoroutine(SpawnCustomerRoutine());
    }

    IEnumerator SpawnCustomerRoutine()
    {
        // ลูปนี้จะทำงานไปเรื่อยๆ ตลอดการเล่นเกม
        while (true)
        {
            // 1. เช็คก่อนว่ามีโต๊ะว่างเหลือ
            Seat[] allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
            int occupiedCount = 0;
            
            foreach (Seat seat in allSeats)
            {
                if (seat.isOccupied) occupiedCount++;
            }

            // 2. ถ้าเก้าอี้ยังไม่เต็มร้านถึงจะเสกลูกค้าใหม่
            if (occupiedCount < allSeats.Length)
            {
                Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("เสกลูกค้าใหม่เข้าร้านแล้ว!");
            }

            // 3. รอเวลา X วินาทีก่อนจะเช็คและเสกคนต่อไป
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}