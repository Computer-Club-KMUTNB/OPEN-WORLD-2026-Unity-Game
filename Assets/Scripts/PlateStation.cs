using UnityEngine;

public class PlateStation : MonoBehaviour
{
    [Header("สิ่งที่อยู่บนจาน")]
    public string item1 = "";
    public string item2 = "";
    public string finalDish = "";

    // ฟังก์ชันรับของจากมือผู้เล่น
    public void AddIngredient(string food, PlayerInteraction player)
    {
        if (finalDish != "") return; // ถ้าอาหารเสร็จแล้วห้ามวางเพิ่ม

        if (item1 == "") 
        {
            item1 = food;
            player.ClearHand(); // สั่งให้มือผู้เล่นว่าง
            Debug.Log("วาง " + food + " ลงจานแล้ว (ชิ้นที่ 1)");
        }
        else if (item2 == "")
        {
            item2 = food;
            player.ClearHand();
            Debug.Log("วาง " + food + " ลงจานแล้ว (ชิ้นที่ 2)");
            
            CheckRecipe(); // ของครบ 2 ชิ้นให้เช็คสูตร
        }
    }

    void CheckRecipe()
    {
        // เช็คสูตรอาหารจากของ 2 สิ่ง (สลับที่กันได้)
        if (HasItems("CookedRice", "CookedPork")) finalDish = "Katsudon";
        else if (HasItems("CookedPork", "ChoppedVeggie")) finalDish = "KoreanPork";
        else if (HasItems("CookedRice", "StewedBeef")) finalDish = "BeefCurry";
        else if (HasItems("GrilledBeef", "ChoppedVeggie")) finalDish = "Steak";
        else 
        {
            // บทลงโทษถ้าผสมมั่ว!
            Debug.Log("ผสมผิดสูตร! จานระเบิด ของหายหมด!");
            item1 = ""; 
            item2 = ""; 
        }

        if (finalDish != "") Debug.Log("✨ ประกอบร่างเสร็จ! กลายเป็น: " + finalDish);
    }

    // เช็คของ 2 อย่างสลับที่กัน
    bool HasItems(string a, string b)
    {
        return (item1 == a && item2 == b) || (item1 == b && item2 == a);
    }

    // ฟังก์ชันให้ผู้เล่นหยิบอาหารที่เสร็จแล้วไปเสิร์ฟ
    public void TakeFinalDish(PlayerInteraction player)
    {
        if (finalDish != "")
        {
            // ค้นหารูปภาพอาหารจากสคริปต์ผู้เล่นแล้วหยิบขึ้นมา
            Sprite dishImage = player.GetFoodSprite(finalDish);
            player.PickUpCookedFood(finalDish, dishImage);
            
            // ล้างจานให้ว่างพร้อมรับออเดอร์ต่อไป
            item1 = "";
            item2 = "";
            finalDish = "";
        }
    }
}