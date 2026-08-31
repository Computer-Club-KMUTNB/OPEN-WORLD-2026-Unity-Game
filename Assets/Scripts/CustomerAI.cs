using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("UI System")]
    public GameObject orderCanvas;
    public Image orderIconImage;
    public Sprite[] foodMenuSprites;
    public string[] foodMenuNames; 
    private string currentOrderName = ""; 

    [Header("ระบบความอดทนและทิป (ใหม่!)")]
    public Image patienceBar;           // หลอดความอดทน (UI Image แบบ Filled)
    public float maxPatienceTime = 30f; // เวลารอสูงสุด
    public float fastServeTime = 10f;   // ถ้าเสิร์ฟทันในเวลานี้ จะได้ทิป
    public int basePrice = 50;          // ราคาอาหารปกติ
    public int tipAmount = 15;          // จำนวนเงินทิป

    private float currentTimer;
    private bool hasOrdered = false;
    private bool hasBeenServed = false;
    private bool isLeaving = false; 

    private Seat mySeat; 
    private Transform exitPoint; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTimer = maxPatienceTime; // เซ็ตเวลาเริ่มต้น
        
        // ระบบค้นหาจุดทางออก
        GameObject exit = GameObject.Find("ExitPoint");
        if (exit != null)
        {
            exitPoint = exit.transform;
        }

        Invoke("FindRandomEmptySeat", 0.1f);
    }

    void Update()
    {
        // 1. เช็คตอนเดินมาถึงโต๊ะ สั่งอาหาร
        if (!hasOrdered && !isLeaving && agent.hasPath)
        {
            if (agent.remainingDistance < 0.5f)
            {
                transform.position = mySeat.transform.position; 
                transform.rotation = mySeat.transform.rotation; 

                hasOrdered = true;
                OrderRandomFood();
            }
        }

        // 2. ระบบนับเวลารออาหาร (ใหม่!)
        if (hasOrdered && !hasBeenServed && !isLeaving)
        {
            currentTimer -= Time.deltaTime; // นับเวลาถอยหลัง

            // อัปเดตหลอดความอดทนบนหัวลูกค้า
            if (patienceBar != null)
            {
                patienceBar.fillAmount = currentTimer / maxPatienceTime;
                
                float timeTaken = maxPatienceTime - currentTimer;

                // เปลี่ยนสีหลอดตามเวลาที่เหลือ
                if (timeTaken <= fastServeTime)
                    patienceBar.color = Color.green; // โซนได้ทิป
                else if (currentTimer > 10f)
                    patienceBar.color = Color.yellow; // โซนปกติ
                else
                    patienceBar.color = Color.red;    // โซนใกล้หมดเวลา (ใกล้โกรธ)
            }

            // ถ้าหมดความอดทน!
            if (currentTimer <= 0)
            {
                LeaveAngry();
            }
        }

        // 3. เดินออกจากร้าน
        if (isLeaving && agent.hasPath)
        {
            if (agent.remainingDistance < 0.5f)
            {
                Debug.Log("ลูกค้าออกจากร้านแล้ว!");
                Destroy(gameObject); 
            }
        }
    }

    void OrderRandomFood()
    {
        if (foodMenuSprites.Length > 0 && orderIconImage != null)
        {
            int randomIndex = Random.Range(0, foodMenuSprites.Length);
            orderIconImage.sprite = foodMenuSprites[randomIndex];
            currentOrderName = foodMenuNames[randomIndex]; 

            if (orderCanvas != null) orderCanvas.SetActive(true);
        }
    }

    void FindRandomEmptySeat()
    {
        if (agent.isOnNavMesh == false) return;

        Seat[] allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None); 
        List<Seat> emptySeats = new List<Seat>();

        foreach (Seat seat in allSeats)
        {
            if (seat.isOccupied == false)
            {
                emptySeats.Add(seat);
            }
        }

        if (emptySeats.Count > 0)
        {
            int randomIndex = Random.Range(0, emptySeats.Count);
            
            mySeat = emptySeats[randomIndex]; 
            mySeat.isOccupied = true;
            
            agent.SetDestination(mySeat.transform.position);
        }
    }

    public bool ReceiveFood(string foodInPlayerHand) 
    {
        if (hasOrdered && !hasBeenServed && !isLeaving)
        {
            if (foodInPlayerHand == currentOrderName) 
            {
                hasBeenServed = true;
                if (orderCanvas != null) orderCanvas.SetActive(false);

                // --- คำนวณทิป (ใหม่!) ---
                float timeTaken = maxPatienceTime - currentTimer;
                int calculatedTip = (timeTaken <= fastServeTime) ? tipAmount : 0;

                if (calculatedTip > 0) Debug.Log($"⚡ เสิร์ฟไวมาก! ได้ทิป {calculatedTip} บาท");
                else Debug.Log("👍 เสิร์ฟทันเวลา ได้ค่าอาหารปกติ");

                // ส่งข้อมูลราคาและทิปไปให้ GameManager บันทึก
                GameManager gm = GameManager.Instance;
                if (gm != null) gm.RecordFoodServed(basePrice, calculatedTip);

                StartCoroutine(EatAndLeave());

                return true;
            }
            else
            {
                Debug.Log("เสิร์ฟผิดเมนู! ฉันสั่ง " + currentOrderName + " แต่คุณเอา " + foodInPlayerHand + " มาให้");
                return false;
            }
        }
        return false;
    }

    IEnumerator EatAndLeave()
    {
        Debug.Log(gameObject.name + " กำลังกินอาหาร...");
        
        yield return new WaitForSeconds(3f);

        if (mySeat != null) mySeat.isOccupied = false;

        isLeaving = true;
        if (exitPoint != null)
        {
            agent.SetDestination(exitPoint.position);
            Debug.Log(gameObject.name + " กินเสร็จแล้ว กำลังเดินออกจากร้าน!");
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    // ฟังก์ชันใหม่: ลูกค้าโกรธเดินหนี
    void LeaveAngry()
    {
        isLeaving = true;
        Debug.Log(gameObject.name + " รอนานเกินไป โมโหเดินออกจากร้านแล้ว!");

        if (orderCanvas != null) orderCanvas.SetActive(false); // ซ่อนป้ายออเดอร์
        
        // สำคัญมาก: ต้องคืนเก้าอี้ให้ว่าง ลูกค้าคนต่อไปจะได้มานั่งโต๊ะนี้ได้!
        if (mySeat != null) mySeat.isOccupied = false;

        if (exitPoint != null)
        {
            agent.SetDestination(exitPoint.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}