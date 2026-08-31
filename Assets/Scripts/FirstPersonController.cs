using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -18f; // Downward gravity to prevent floating

    public Transform playerCamera;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) playerCamera = cam.transform;
            else if (Camera.main != null) playerCamera = Camera.main.transform;
        }

        LockCursor();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && Time.timeScale > 0f)
        {
            LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. FREEZE EVERYTHING WHEN PAUSED (Cannot move camera or walk)
        if (Time.timeScale <= 0f)
        {
            return;
        }

        // Re-lock cursor when player clicks into the game window while unpaused
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKeyDown)
            {
                LockCursor();
            }
        }

        // Camera look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        if (playerCamera != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        // Walking Movement with Gravity
        if (controller != null)
        {
            float moveForward = Input.GetAxis("Vertical");
            float moveSide = Input.GetAxis("Horizontal");

            Vector3 move = transform.right * moveSide + transform.forward * moveForward;

            if (controller.isGrounded)
            {
                if (verticalVelocity < 0f)
                {
                    verticalVelocity = -3f;
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 velocityVector = (move * walkSpeed) + (Vector3.up * verticalVelocity);
            controller.Move(velocityVector * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        // Ensure cursor is locked and hidden whenever unpaused and pause menus are not loaded
        if (Time.timeScale > 0f)
        {
            Scene pauseCute = SceneManager.GetSceneByName("PauseMenu_Cute");
            Scene pauseHunt = SceneManager.GetSceneByName("PauseMenu_Hunt");
            if (!pauseCute.isLoaded && !pauseHunt.isLoaded)
            {
                if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}