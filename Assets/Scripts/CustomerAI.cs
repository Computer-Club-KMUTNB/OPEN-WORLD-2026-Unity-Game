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
    public string[] foodMenuNames; // เก็บชื่อของเมนูให้ตรงกับรูปภาพ
    private string currentOrderName = ""; // ลูกค้าสั่งเมนูชื่ออะไร

    private bool hasOrdered = false;
    private bool hasBeenServed = false;
    private bool isLeaving = false; // เอาไว้เช็คว่ากำลังเดินออกจากร้าน

    private Seat mySeat; // ลูกค้านั่งเก้าอี้ตัวไหนอยู่
    private Transform exitPoint; // ทางออก

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // ะบบค้นหาจุดทางออก
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

        // 2. เดินออกจากร้าน
        if (isLeaving && agent.hasPath)
        {
            if (agent.remainingDistance < 0.5f)
            {
                Debug.Log("ลูกค้าออกจากร้านแล้ว!");
                Destroy(gameObject); // ลบตัวลูกค้าทิ้ง
            }
        }
    }

    void OrderRandomFood()
    {
        if (foodMenuSprites.Length > 0 && orderIconImage != null)
        {
            int randomIndex = Random.Range(0, foodMenuSprites.Length);
            orderIconImage.sprite = foodMenuSprites[randomIndex];
            
            // ลูกค้าจำชื่อเมนูจากลำดับเดียวกันกับรูปภาพ
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
            
            // จำเก้าอี้ที่ตัวเองเลือกไว้และเปลี่ยนสถานะเป็น "มีคนนั่ง"
            mySeat = emptySeats[randomIndex]; 
            mySeat.isOccupied = true;
            
            agent.SetDestination(mySeat.transform.position);
        }
    }

    public bool ReceiveFood(string foodInPlayerHand) 
    {
        if (hasOrdered && !hasBeenServed)
        {
            // เช็คว่าของในมือผู้เล่นตรงกับเมนูที่สั่ง
            if (foodInPlayerHand == currentOrderName) 
            {
                hasBeenServed = true;
                if (orderCanvas != null) orderCanvas.SetActive(false);

                GameManager gm = FindAnyObjectByType<GameManager>();
                if (gm != null) gm.ServeFood();

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

    // ฟังก์ชันรอเวลากินอาหาร
    IEnumerator EatAndLeave()
    {
        Debug.Log(gameObject.name + " กำลังกินอาหาร...");
        
        // รอเวลาลูกค้ากินอาหาร 3 วินาที
        yield return new WaitForSeconds(3f);

        // คืนสถานะเก้าอี้ให้ว่าง
        if (mySeat != null)
        {
            mySeat.isOccupied = false;
        }

        // ดินไปที่ประตูทางออก
        isLeaving = true;
        if (exitPoint != null)
        {
            agent.SetDestination(exitPoint.position);
            Debug.Log(gameObject.name + " กินเสร็จแล้ว กำลังเดินออกจากร้าน!");
        }
        else
        {
            Destroy(gameObject); // ถ้าหาทางออกไม่เจอ ก็ลบตัวเองทิ้งไปเลย
        }
    }
}