using UnityEngine;
using System.Collections;

public class ShopBell : MonoBehaviour
{
    [Header("อ้างอิงถึงตัวคุมเวลา")]
    public DayTimer dayTimer;

    [Header("เสียงกระดิ่ง (Optional)")]
    public AudioSource audioSource;
    public AudioClip bellSound;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // ถ้าไม่ได้ลาก DayTimer มาใส่ ให้ค้นหาอัตโนมัติ
        if (dayTimer == null)
        {
            dayTimer = FindFirstObjectByType<DayTimer>();
        }
    }

    // ฟังก์ชันเมื่อผู้เล่นยิงเลเซอร์มาคลิกที่กระดิ่ง
    public void RingBell()
    {
        if (dayTimer == null) return;

        // เล่นเสียงกระดิ่ง (ถ้ามี)
        if (audioSource != null && bellSound != null)
        {
            audioSource.PlayOneShot(bellSound);
        }

        // เอฟเฟกต์กระดิ่งยุบแล้วเด้งคืน
        StartCoroutine(SquishEffect());

        // สลับสถานะ เปิด/ปิด ร้าน
        if (!dayTimer.isShopOpen)
        {
            dayTimer.OpenShop();
            Debug.Log("🔔 กริ๊งงง! เปิดร้านแล้ว!");
        }
        else
        {
            dayTimer.CloseShop();
            Debug.Log("🔔 กริ๊งงง! ปิดร้านแล้ว!");
        }
    }

    // แอนิเมชันย่อ-ขยายสั้นๆ เวลาถูกกด
    IEnumerator SquishEffect()
    {
        transform.localScale = originalScale * 0.85f; // หดตัวลงเล็กน้อย
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale; // เด้งกลับขนาดเดิม
    }
}