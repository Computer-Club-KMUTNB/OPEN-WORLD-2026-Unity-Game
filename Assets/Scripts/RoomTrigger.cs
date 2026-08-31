using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("Room Setup")]
    [Tooltip("ลาก EnemySpawner ของห้องนี้มาใส่")]
    public EnemySpawner roomSpawner;
    [Tooltip("ลาก ประตูทางเข้า มาใส่ (ถ้าต้องการให้ปิดล็อกขังผู้เล่น)")]
    public DoorController entranceDoor;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจจับเฉพาะเมื่อเป็นตัว Player และยังไม่เคยทริกเกอร์
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("🚶 Player เดินเข้าห้อง เริ่มต้นต่อสู้!");

            // ปิดประตูด้านหลังล็อกขังไว้ (ถ้ามี)
            if (entranceDoor != null)
            {
                entranceDoor.CloseDoor();
            }

            // สั่งให้ Spawner เริ่มทำงาน
            if (roomSpawner != null)
            {
                roomSpawner.StartWave();
            }

            // ปิด Collider ตัวนี้เพื่อไม่ให้ทำงานซ้ำ
            GetComponent<Collider>().enabled = false;
        }
    }
}