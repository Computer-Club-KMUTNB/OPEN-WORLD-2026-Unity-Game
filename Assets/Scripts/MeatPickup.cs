using UnityEngine;

public class MeatPickup : MonoBehaviour
{
    public float rotateSpeed = 60f;

    void Update()
    {
        // หมุนไอเทมรอบแกน Y เพื่อเอฟเฟกต์สวยงาม
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
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
        // สั่งให้ InventoryManager เพิ่มเนื้อ 1 ชิ้น
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddMeat(1);
        }

        Destroy(gameObject);
    }
}