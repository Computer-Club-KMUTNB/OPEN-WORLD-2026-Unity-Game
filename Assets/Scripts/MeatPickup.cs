using UnityEngine;

public class MeatPickup : MonoBehaviour
{
    public enum MeatKind { RawBeef, RawPork }
    
    [Header("Item Type")]
    public MeatKind meatKind = MeatKind.RawBeef;
    public int amount = 1;
    public float rotateSpeed = 60f;

    void Update()
    {
        // หมุนไอเทมรอบแกน Y เพื่อเอฟเฟกต์สวยงาม
        if (rotateSpeed > 0f)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        if (InventoryManager.Instance != null)
        {
            bool isPork = (meatKind == MeatKind.RawPork) || gameObject.name.ToLower().Contains("pork");
            if (isPork)
            {
                InventoryManager.Instance.AddPork(amount);
            }
            else
            {
                InventoryManager.Instance.AddMeat(amount);
            }
        }

        Destroy(gameObject);
    }
}