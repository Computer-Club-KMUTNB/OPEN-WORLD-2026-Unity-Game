using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 baseLocalPosition;
    private bool isShaking = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // บันทึกความสูงและตำแหน่งเริ่มต้นของกล้อง
        baseLocalPosition = transform.localPosition;
    }

    public void Shake(float duration = 0.2f, float positionMagnitude = 0.08f, float rotationMagnitude = 2.0f)
    {
        if (!gameObject.activeInHierarchy) return;

        StopAllCoroutines();
        StartCoroutine(DoShake(duration, positionMagnitude, rotationMagnitude));
    }

    IEnumerator DoShake(float duration, float posMag, float rotMag)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // สุ่มแรงสั่น (ลดแรงในแกน Y เพื่อไม่ให้กล้องเด้งสูงเกินไป)
            float x = Random.Range(-1f, 1f) * posMag;
            float y = Random.Range(-0.5f, 0.5f) * (posMag * 0.4f);
            float zRot = Random.Range(-1f, 1f) * rotMag;

            // กำหนดตำแหน่งโดยอิงจาก baseLocalPosition เสมอ (ไม่บวกสะสม)
            transform.localPosition = baseLocalPosition + new Vector3(x, y, 0f);
            transform.localRotation *= Quaternion.Euler(0f, 0f, zRot);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // คืนค่าตำแหน่งความสูงเดิมของกล้องเป๊ะๆ 100%
        transform.localPosition = baseLocalPosition;
        isShaking = false;
    }
}