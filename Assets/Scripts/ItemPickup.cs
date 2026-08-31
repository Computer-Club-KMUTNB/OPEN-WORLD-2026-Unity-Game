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
    [Tooltip("ระยะแม่เหล็กดูดเก็บไอเทมอัตโนมัติเมื่อผู้เล่นเดินเข้าใกล้")]
    public float pickupRadius = 2.4f;

    private bool canPickup = false;
    private bool hasPickedUp = false;
    private Collider itemCollider;
    private Transform playerTransform;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        Invoke(nameof(EnablePickup), pickupDelay);
        FindPlayer();
    }

    private void EnablePickup()
    {
        canPickup = true;
    }

    private void FindPlayer()
    {
        PlayerHealth ph = FindAnyObjectByType<PlayerHealth>();
        if (ph != null) playerTransform = ph.transform;
        else
        {
            FirstPersonController fpc = FindAnyObjectByType<FirstPersonController>();
            if (fpc != null) playerTransform = fpc.transform;
            else if (Camera.main != null) playerTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        if (!canPickup || hasPickedUp) return;

        // Proximity Pickup Check (รับประกันการเก็บเนื้อได้ 100% แม้ Trigger Collider จะมีขนาดต่างกัน)
        if (playerTransform == null) FindPlayer();
        if (playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist <= pickupRadius)
            {
                CollectItem();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup || hasPickedUp) return;
        if (IsPlayer(other))
        {
            CollectItem();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canPickup || hasPickedUp) return;
        if (IsPlayer(other))
        {
            CollectItem();
        }
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null) return false;
        if (other.CompareTag("Player")) return true;
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")) return true;
        if (other.GetComponentInParent<PlayerHealth>() != null) return true;
        if (other.GetComponentInParent<FirstPersonController>() != null) return true;
        if (other.name.ToLower().Contains("player")) return true;
        return false;
    }

    private void CollectItem()
    {
        if (hasPickedUp) return;
        hasPickedUp = true;

        if (itemCollider != null) itemCollider.enabled = false;

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

        Destroy(gameObject);
    }
}