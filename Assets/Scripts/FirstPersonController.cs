using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    private CharacterController controller;
    private float verticalRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // ซ่อนเมาส์และล็อคเป้าไว้ตรงกลางจอ
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    void Update()
    {
        // ระบบหันมุมกล้องด้วยเมาส์
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // หันซ้าย-ขวา (หมุนทั้งตัวผู้เล่น)
        transform.Rotate(Vector3.up * mouseX);

        // ก้ม-เงย (หมุนเฉพาะกล้อง)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // ล็อคคอไม่ให้หักหมุนได้แค่ 90 องศา
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // ระบบเดินด้วยปุ่ม WASD
        float moveForward = Input.GetAxis("Vertical");
        float moveSide = Input.GetAxis("Horizontal");

        Vector3 move = transform.right * moveSide + transform.forward * moveForward;
        controller.Move(move * walkSpeed * Time.deltaTime);
    }
}