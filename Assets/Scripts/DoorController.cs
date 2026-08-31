using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Movement")]
    public Vector3 openOffset = new Vector3(0, 4f, 0); // ระยะที่ประตูจะเลื่อนขึ้น
    public float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void OpenDoor()
    {
        targetPosition = closedPosition + openOffset;
        isMoving = true;
        Debug.Log("🚪 ประตูเปิดออกแล้ว!");
    }

    public void CloseDoor()
    {
        targetPosition = closedPosition;
        isMoving = true;
        Debug.Log("🔒 ประตูปิดล็อก!");
    }
}