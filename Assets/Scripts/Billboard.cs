using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // search for player cam
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Canvas always face player
        transform.LookAt(transform.position + mainCameraTransform.forward);
    }
}