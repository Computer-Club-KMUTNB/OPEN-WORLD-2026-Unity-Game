using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Movement")]
    [Tooltip("ระยะเลื่อนเปิดแบบ vector (ถ้าใช้ได้) ")]
    public Vector3 openOffset = new Vector3(0, 4f, 0);
    [Tooltip("ระยะความสูงที่ประตูจะเลื่อนขึ้นเมื่อไม่ได้ใช้ openOffset")]
    public float openHeight = 3.5f;
    [Tooltip("ความเร็วในการเลื่อนเปิด/ปิด")]
    public float openSpeed = 3f;

    [Header("Auto Destroy")]
    [Tooltip("ลบ GameObject ประตูทิ้งเมื่อเปิดเสร็จแล้ว")]
    public bool destroyOnOpen = true;
    [Tooltip("เวลารอก่อนลบ (วินาที) หลังจากประตูเลื่อนเปิดสุดแล้ว (0 = ลบหลังจากเปิดสุดทันที)")]
    public float destroyDelay = 0.2f;

    [Header("Camera Shake Settings")]
    public bool shakeCameraOnOpen = true;
    public float shakeDuration = 1.2f;
    public float shakePosMagnitude = 0.04f;
    public float shakeRotMagnitude = 0.8f;

    [Header("Audio (Optional)")]
    public AudioSource doorAudioSource;
    public AudioClip stoneRumbleSound;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Awake()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;

            if (isOpen)
            {
                HandleOpened();
            }
        }
    }

    private void HandleOpened()
    {
        if (destroyOnOpen)
        {
            // ปิด Collider ทั้งหมดทันที เพื่อไม่ให้ขวางทางผู้เล่น
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }

            GameObject rootToDestroy = gameObject;
            if (transform.parent != null && (transform.parent.name.Contains("Door") || transform.parent.name.Contains("door")))
            {
                rootToDestroy = transform.parent.gameObject;
            }

            Debug.Log($"🗑️ ประตู {rootToDestroy.name} เปิดแล้ว ทำการลบออกจากฉาก...");
            Destroy(rootToDestroy, Mathf.Max(0f, destroyDelay));
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        closedPosition = transform.position;

        Vector3 offset = (openOffset != Vector3.zero) ? openOffset : Vector3.up * Mathf.Max(openHeight, 0.1f);
        targetPosition = closedPosition + offset;
        isMoving = true;

        if (doorAudioSource != null && stoneRumbleSound != null)
        {
            doorAudioSource.PlayOneShot(stoneRumbleSound);
        }

        if (shakeCameraOnOpen && CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakePosMagnitude, shakeRotMagnitude);
        }

        Debug.Log($"🚪 ประตู {gameObject.name} เปิดออกแล้ว!");

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f || openSpeed <= 0f)
        {
            transform.position = targetPosition;
            isMoving = false;
            HandleOpened();
        }
    }

    public void CloseDoor()
    {
        if (!isOpen) return;
        isOpen = false;

        targetPosition = closedPosition;
        isMoving = true;
        Debug.Log("🔒 ประตูปิดล็อก!");
    }
}