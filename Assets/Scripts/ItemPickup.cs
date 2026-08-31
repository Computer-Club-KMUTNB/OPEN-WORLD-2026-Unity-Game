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
    [Tooltip("หน่วงก่อนให้เก็บไอเทมได้")]
    public float pickupDelay = 0.25f;

    private bool canPickup = false;
    private bool hasPickedUp = false;

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
        if (!canPickup || hasPickedUp) return;

        bool isPlayer = other.CompareTag("Player");
        if (!isPlayer && other.attachedRigidbody != null)
        {
            isPlayer = other.attachedRigidbody.CompareTag("Player");
        }

        if (!isPlayer) return;

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
        else
        {
            Debug.LogError("❌ ไม่พบ InventoryManager ในฉาก!");
            return;
        }

        hasPickedUp = true;
        Destroy(gameObject);
    }
}