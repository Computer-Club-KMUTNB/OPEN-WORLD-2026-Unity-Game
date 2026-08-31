using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        Meat, // Raw Beef
        Pork  // Raw Pork
    }

    [Header("Item Configuration")]
    [Tooltip("เลือกชนิดไอเทมให้ตรงกับ Prefab")]
    public ItemType itemType = ItemType.Meat;
    [Tooltip("จำนวนที่ได้รับต่อการเก็บ 1 ครั้ง")]
    public int amount = 1;

    [Header("Visual Animation")]
    [Tooltip("ความเร็วในการหมุนไอเทม")]
    public float rotateSpeed = 90f;

    [Header("Pickup Settings")]
    [Tooltip("หน่วงก่อนให้เก็บไอเทมได้")]
    public float pickupDelay = 0.25f;
    public float pickupRadius = 2.5f;

    private bool canPickup = false;
    private bool hasPickedUp = false;
    private Transform playerTransform;
    private Vector3 basePosition;
    private float bobTimer = 0f;

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, 15f))
        {
            transform.position = hit.point + Vector3.up * 0.6f;
        }

        basePosition = transform.position;
        bobTimer = Random.Range(0f, 6.28f);

        Invoke(nameof(EnablePickup), pickupDelay);
        FindPlayer();
    }

    private void EnablePickup()
    {
        canPickup = true;
    }

    void Update()
    {
        if (hasPickedUp) return;

        bobTimer += Time.deltaTime * 3f;
        float bobOffset = Mathf.Sin(bobTimer) * 0.12f;
        transform.position = new Vector3(basePosition.x, basePosition.y + bobOffset, basePosition.z);

        if (rotateSpeed > 0f)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        if (canPickup)
        {
            if (playerTransform == null) FindPlayer();
            if (playerTransform != null)
            {
                float dist = Vector3.Distance(transform.position, playerTransform.position);
                if (dist <= pickupRadius)
                {
                    Collect();
                }
            }
        }
    }

    private void FindPlayer()
    {
        PlayerHealth ph = FindAnyObjectByType<PlayerHealth>();
        if (ph != null) playerTransform = ph.transform;

        if (playerTransform == null)
        {
            FirstPersonController fpc = FindAnyObjectByType<FirstPersonController>();
            if (fpc != null) playerTransform = fpc.transform;
        }

        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        if (playerTransform == null && Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup || hasPickedUp) return;

        bool isPlayer = other.CompareTag("Player") ||
                        (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")) ||
                        other.GetComponentInParent<PlayerHealth>() != null ||
                        other.name.ToLower().Contains("player");

        if (isPlayer)
        {
            Collect();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canPickup || hasPickedUp) return;

        bool isPlayer = collision.gameObject.CompareTag("Player") ||
                        collision.gameObject.GetComponentInParent<PlayerHealth>() != null;

        if (isPlayer)
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (hasPickedUp) return;
        hasPickedUp = true;

        bool isPork = (itemType == ItemType.Pork) || gameObject.name.ToLower().Contains("pork");

        if (isPork)
        {
            InventoryManager.globalPorkCount += amount;
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddPork(amount);
            }
            Debug.Log($"🍖 Pork collected +{amount}! Total: {InventoryManager.globalPorkCount}");
        }
        else
        {
            InventoryManager.globalMeatCount += amount;
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddMeat(amount);
            }
            Debug.Log($"🥩 Meat collected +{amount}! Total: {InventoryManager.globalMeatCount}");
        }

        Destroy(gameObject);
    }
}