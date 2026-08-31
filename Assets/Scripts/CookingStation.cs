using UnityEngine;
using System.Collections;

public class CookingStation : MonoBehaviour
{
    [Header("Recipe Settings (สูตรอาหาร)")]
    public string requiredIngredient; // ของดิบที่ต้องใช้
    public string resultFoodName;     // ชื่ออาหารที่ทำเสร็จ
    public Sprite resultFoodSprite;   // รูปของอาหารที่ทำเสร็จแล้ว
    public float cookTime = 3f;       // เวลาทำอาหาร

    [Header("Status (สถานะเตา)")]
    public bool isCooking = false;
    public bool hasFinishedFood = false;

    // ฟังก์ชันเริ่มทำอาหาร
    public void StartCooking()
    {
        if (!isCooking && !hasFinishedFood)
        {
            StartCoroutine(CookRoutine());
        }
    }

    IEnumerator CookRoutine()
    {
        isCooking = true;
        Debug.Log(gameObject.name + " กำลังทำงาน...");
        
        // รอเวลาทำอาหาร
        yield return new WaitForSeconds(cookTime);
        
        isCooking = false;
        hasFinishedFood = true;
        Debug.Log(gameObject.name + " ทำอาหารเสร็จแล้ว พร้อมเสิร์ฟ " + resultFoodName);
    }

    // ฟังก์ชันสำหรับให้ผู้เล่นหยิบอาหารที่เสร็จแล้วออกจากเตา
    public void TakeFinishedFood(PlayerInteraction player)
    {
        if (hasFinishedFood)
        {
            hasFinishedFood = false;
            // ส่งชื่อและรูปอาหารไปใส่มือผู้เล่น
            player.PickUpCookedFood(resultFoodName, resultFoodSprite); 
        }
    }
}
