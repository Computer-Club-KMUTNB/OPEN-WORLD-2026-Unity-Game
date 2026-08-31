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

    [Header("ระบบความอดทนและทิป")]
    public Image patienceBar;           // หลอดความอดทน (UI Image แบบ Filled)
    public float maxPatienceTime = 75f; // เวลารอสูงสุด (เพิ่มเวลาให้เสิร์ฟสบายขึ้น)
    public float fastServeTime = 25f;   // ถ้าเสิร์ฟทันในเวลานี้ จะได้ทิปพิเศษ
    public int basePrice = 50;          // ราคาอาหารปกติ
    public int tipAmount = 20;          // จำนวนเงินทิป

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
                if (mySeat != null)
                {
                    transform.position = mySeat.transform.position; 
                    transform.rotation = mySeat.transform.rotation; 
                }

                hasOrdered = true;
                OrderRandomFood();
            }
        }

        // 2. ระบบนับเวลารออาหาร
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
                    patienceBar.color = new Color(0.3f, 0.9f, 0.4f); // โซนได้ทิป (เขียวสด)
                else if (currentTimer > 20f)
                    patienceBar.color = new Color(1.0f, 0.85f, 0.25f); // โซนปกติ (เหลือง)
                else
                    patienceBar.color = new Color(1.0f, 0.35f, 0.35f);    // โซนใกล้หมดเวลา (แดง)
            }

            // ถ้าหมดความอดทน
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
        if (foodMenuSprites != null && foodMenuSprites.Length > 0 && orderIconImage != null)
        {
            int randomIndex = Random.Range(0, foodMenuSprites.Length);
            orderIconImage.sprite = foodMenuSprites[randomIndex];
            if (foodMenuNames != null && randomIndex < foodMenuNames.Length)
            {
                currentOrderName = foodMenuNames[randomIndex];
            }

            if (orderCanvas != null) orderCanvas.SetActive(true);
        }
    }

    void FindRandomEmptySeat()
    {
        if (agent == null || agent.isOnNavMesh == false) return;

        Seat[] allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None); 
        List<Seat> emptySeats = new List<Seat>();

        if (allSeats != null)
        {
            foreach (Seat seat in allSeats)
            {
                if (seat != null && seat.isOccupied == false)
                {
                    emptySeats.Add(seat);
                }
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

                // คำนวณทิป
                float timeTaken = maxPatienceTime - currentTimer;
                int calculatedTip = (timeTaken <= fastServeTime) ? tipAmount : 0;

                if (calculatedTip > 0) Debug.Log($"⚡ เสิร์ฟไวมาก! ได้ทิป {calculatedTip} บาท");
                else Debug.Log("👍 เสิร์ฟทันเวลา ได้ค่าอาหารปกติ");

                GameManager gm = GameManager.Instance;
                if (gm != null) gm.RecordFoodServed(basePrice, calculatedTip);

                if (RestaurantFlowController.Instance != null)
                {
                    RestaurantFlowController.Instance.RegisterCustomerServed(true);
                }

                StartCoroutine(EatAndLeave());
                return true;
            }
            else
            {
                if (foodInPlayerHand == "BurntMess")
                {
                    Debug.Log("🤢 ลูกค้าโมโหที่ได้อาหารไหม้! เสียความอดทนทันที");
                    currentTimer = Mathf.Max(0f, currentTimer - 25f);
                }
                else
                {
                    Debug.Log($"เสิร์ฟผิดเมนู! สั่ง: {currentOrderName} แต่ได้: {foodInPlayerHand}");
                }
                return false;
            }
        }
        return false;
    }

    IEnumerator EatAndLeave()
    {
        yield return new WaitForSeconds(3f);

        if (mySeat != null) mySeat.isOccupied = false;

        isLeaving = true;
        if (exitPoint != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(exitPoint.position);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void LeaveAngry()
    {
        isLeaving = true;
        Debug.Log("ลูกค้าหมดความอดทนเดินออกจากร้าน!");

        if (orderCanvas != null) orderCanvas.SetActive(false);
        if (mySeat != null) mySeat.isOccupied = false;

        if (exitPoint != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(exitPoint.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}