using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        Meat,
        Pork
    }

    [Header("Item Configuration")]
    [Tooltip("เลือกชนิดไอเทมให้ตรงกับ Prefab")]
    public ItemType itemType;
    [Tooltip("จำนวนที่ได้รับต่อการเก็บ 1 ครั้ง")]
    public int amount = 1;

    [Header("Visual Animation")]
    [Tooltip("ความเร็วในการหมุนไอเทม")]
    public float rotateSpeed = 90f;

    [Header("Pickup Settings")]
    [Tooltip("หน่วงเวลาเล็กน้อยก่อนเปิดให้เก็บ")]
    public float pickupDelay = 0.2f;

    private bool canPickup = false;
    private bool hasPickedUp = false;
    private Collider itemCollider;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    private void EnablePickup()
    {
        canPickup = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ถ้ายังไม่เปิดให้เก็บ หรือถูกเก็บไปแล้วในเฟรมนี้ ให้หยุดทันที
        if (!canPickup || hasPickedUp) return;

        // เช็กว่าเป็นตัวละคร Player
        bool isPlayer = other.CompareTag("Player");
        if (!isPlayer && other.attachedRigidbody != null)
        {
            isPlayer = other.attachedRigidbody.CompareTag("Player");
        }

        if (!isPlayer) return;

        // ⚠️ ล็อกสถานะทันที และปิด Collider ของตัวไอเทมเพื่อไม่ให้ Collider ชิ้นที่ 2 ของ Player มาโดนซ้ำ
        hasPickedUp = true;
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        InventoryManager manager = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : FindFirstObjectByType<InventoryManager>();

        if (manager != null)
        {
            if (itemType == ItemType.Meat)
            {
                manager.AddMeat(amount);
            }
            else if (itemType == ItemType.Pork)
            {
                manager.AddPork(amount);
            }
        }

        // ลบวัตถุออกจากฉาก
        Destroy(gameObject);
    }
}