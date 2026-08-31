using UnityEngine;

public class MeatPickup : MonoBehaviour
{
    public enum MeatKind { RawBeef, RawPork }
    
    [Header("Item Type")]
    public MeatKind meatKind = MeatKind.RawBeef;
    public int amount = 1;
    public float rotateSpeed = 90f;
    public float pickupRadius = 2.5f;

    private Transform playerTransform;
    private bool isCollected = false;
    private Vector3 basePosition;
    private float bobTimer = 0f;

    private void Awake()
    {
        // Disable gravity on any Rigidbody immediately so it NEVER falls through the floor
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Set trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Start()
    {
        // Raycast down to find exact ground surface so the meat sits nicely above ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, 15f))
        {
            transform.position = hit.point + Vector3.up * 0.6f;
        }

        basePosition = transform.position;
        bobTimer = Random.Range(0f, 6.28f);

        FindPlayer();
    }

    private void Update()
    {
        if (isCollected) return;

        // 1. Hover & Rotate animation
        bobTimer += Time.deltaTime * 3f;
        float bobOffset = Mathf.Sin(bobTimer) * 0.12f;
        transform.position = new Vector3(basePosition.x, basePosition.y + bobOffset, basePosition.z);

        if (rotateSpeed > 0f)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }

        // 2. Magnet Proximity Pickup
        if (playerTransform == null)
        {
            FindPlayer();
        }

        if (playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist <= pickupRadius)
            {
                CollectItem();
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
        if (isCollected) return;

        if (IsPlayerCollider(other.gameObject))
        {
            CollectItem();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCollected) return;

        if (IsPlayerCollider(collision.gameObject))
        {
            CollectItem();
        }
    }

    private bool IsPlayerCollider(GameObject obj)
    {
        if (obj == null) return false;
        if (obj.CompareTag("Player")) return true;
        if (obj.GetComponentInParent<PlayerHealth>() != null) return true;
        if (obj.GetComponentInParent<FirstPersonController>() != null) return true;
        if (obj.GetComponentInParent<CharacterController>() != null) return true;
        if (obj.name.ToLower().Contains("player")) return true;
        return false;
    }

    public void CollectItem()
    {
        if (isCollected) return;
        isCollected = true;

        bool isPork = (meatKind == MeatKind.RawPork) || gameObject.name.ToLower().Contains("pork");
        
        if (isPork)
        {
            InventoryManager.globalPorkCount += amount;
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.UpdateMeatUI();
            }
            Debug.Log($"🥓 Raw Pork looted +{amount}! Total in Inventory: {InventoryManager.globalPorkCount}");
        }
        else
        {
            InventoryManager.globalMeatCount += amount;
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.UpdateMeatUI();
            }
            Debug.Log($"🥩 Raw Beef looted +{amount}! Total in Inventory: {InventoryManager.globalMeatCount}");
        }

        Destroy(gameObject);
    }
}