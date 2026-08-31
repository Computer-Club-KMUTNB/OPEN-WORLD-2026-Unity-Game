using UnityEngine;

public class PlateStation : MonoBehaviour
{
    [Header("สิ่งที่อยู่บนจาน")]
    public string item1 = "";
    public string item2 = "";
    public string finalDish = "";

    [Header("จุดวางโมเดล 3D บนโต๊ะ")]
    public Transform item1Slot;     // จุดวางของชิ้นที่ 1
    public Transform item2Slot;     // จุดวางของชิ้นที่ 2
    public Transform finalDishSlot; // จุดวางจานที่ทำเสร็จแล้ว (ตรงกลาง)

    [Header("คลังโมเดล 3D")]
    public GameObject[] all3DModels;
    public string[] all3DModelNames;

    // ตัวแปรเก็บโมเดล 3D ที่กำลังโชร์อยู่บนโต๊ะ
    private GameObject model1;
    private GameObject model2;
    private GameObject finalModel;

    // ฟังก์ชันรับของจากมือผู้เล่น
    public void AddIngredient(string food, PlayerInteraction player)
    {
        if (finalDish != "") return; // ถ้าอาหารเสร็จแล้ว ห้ามวางเพิ่ม

        if (item1 == "") 
        {
            item1 = food;
            player.ClearHand(); // สั่งเคลียร์ของออกจากมือผู้เล่น
            model1 = SpawnModel(item1, item1Slot != null ? item1Slot : transform);
            Debug.Log("วาง " + food + " ลงจานแล้ว (ชิ้นที่ 1)");
        }
        else if (item2 == "")
        {
            item2 = food;
            player.ClearHand();
            model2 = SpawnModel(item2, item2Slot != null ? item2Slot : transform);
            Debug.Log("วาง " + food + " ลงจานแล้ว (ชิ้นที่ 2)");
            
            CheckRecipe(); // ของครบ 2 ชิ้น ให้เช็คสูตรทันที!
        }
    }

    void CheckRecipe()
    {
        // เช็คสูตรอาหาร
        if (HasItems("CookedRice", "CookedPork")) finalDish = "Katsudon";
        else if (HasItems("CookedPork", "ChoppedVeggie")) finalDish = "KoreanPork";
        else if (HasItems("CookedRice", "StewedBeef")) finalDish = "BeefCurry";
        else if (HasItems("GrilledBeef", "ChoppedVeggie")) finalDish = "Steak";
        else 
        {
            Debug.Log("ผสมผิดสูตร! จานระเบิด ของหายหมด!");
            ClearAllModels();
            item1 = ""; 
            item2 = ""; 
            return;
        }

        if (finalDish != "") 
        {
            Debug.Log("✨ ประกอบร่างเสร็จ! กลายเป็น: " + finalDish);
            
            // ลบโมเดลของดิบ/ของสุกทั้ง 2 ชิ้นทิ้ง
            if (model1 != null) Destroy(model1);
            if (model2 != null) Destroy(model2);

            // เสกโมเดลอาหารสำเร็จรูปขึ้นมาตรงกลาง
            finalModel = SpawnModel(finalDish, finalDishSlot != null ? finalDishSlot : transform);
        }
    }

    bool HasItems(string a, string b)
    {
        return (item1 == a && item2 == b) || (item1 == b && item2 == a);
    }

    // ฟังก์ชันช่วยเสกโมเดล 3D ลงตำแหน่ง Slot
    GameObject SpawnModel(string itemName, Transform targetSlot)
    {
        for (int i = 0; i < all3DModelNames.Length; i++)
        {
            if (all3DModelNames[i] == itemName && all3DModels[i] != null)
            {
                GameObject newModel = Instantiate(all3DModels[i], targetSlot.position, targetSlot.rotation);
                newModel.transform.SetParent(targetSlot);
                
                // ปิดฟิสิกส์กันโมเดลกระเด็นตกโต๊ะ
                Collider col = newModel.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                Rigidbody rb = newModel.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                return newModel;
            }
        }
        return null;
    }

    // ฟังก์ชันให้ผู้เล่นหยิบอาหารที่เสร็จแล้วไปเสิร์ฟ
    public void TakeFinalDish(PlayerInteraction player)
    {
        if (finalDish != "")
        {
            Sprite dishImage = player.GetFoodSprite(finalDish);
            player.PickUpCookedFood(finalDish, dishImage);
            
            // ล้างโมเดลบนโต๊ะออกให้หมด
            ClearAllModels();
            item1 = "";
            item2 = "";
            finalDish = "";
        }
    }

    void ClearAllModels()
    {
        if (model1 != null) Destroy(model1);
        if (model2 != null) Destroy(model2);
        if (finalModel != null) Destroy(finalModel);
    }
}