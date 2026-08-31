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
    private Coroutine moveCoroutine;

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

        Debug.Log("🚪 ประตูเปิดออกแล้ว!");
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