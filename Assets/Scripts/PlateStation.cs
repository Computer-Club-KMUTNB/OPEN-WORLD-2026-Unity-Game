using UnityEngine;
using System.Collections;

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
            player.ClearHand();
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
        // 1. เช็คสูตรอาหารที่ถูกต้อง
        if (HasItems("CookedRice", "CookedPork")) finalDish = "Katsudon";
        else if (HasItems("CookedPork", "ChoppedVeggie")) finalDish = "KoreanPork";
        else if (HasItems("CookedRice", "StewedBeef")) finalDish = "BeefCurry";
        else if (HasItems("GrilledBeef", "ChoppedVeggie")) finalDish = "Steak";
        else 
        {
            // 2. ผสมผิดสูตร! กลายเป็นอาหารไหม้ / Burnt Mess
            finalDish = "BurntMess";
            Debug.Log("💥 ผสมผิดสูตร! กลายเป็นอาหารไหม้ (Burnt Mess) — นำไปทิ้งถังขยะหรือทิ้งด้วยปุ่ม [Q]");

            if (model1 != null) Destroy(model1);
            if (model2 != null) Destroy(model2);

            finalModel = SpawnModel("BurntMess", finalDishSlot != null ? finalDishSlot : transform);
            if (finalModel == null && all3DModels != null && all3DModels.Length > 0)
            {
                // เสกโมเดลสีดำไหม้เพื่อแสดงผลว่าเป็นของเสีย
                GameObject template = all3DModels[all3DModels.Length - 1] ?? all3DModels[0];
                if (template != null)
                {
                    Transform slot = finalDishSlot != null ? finalDishSlot : transform;
                    finalModel = Instantiate(template, slot.position, slot.rotation);
                    finalModel.transform.SetParent(slot);

                    Renderer[] renderers = finalModel.GetComponentsInChildren<Renderer>();
                    foreach (var r in renderers)
                    {
                        if (r != null && r.material != null)
                        {
                            r.material.color = new Color(0.18f, 0.12f, 0.12f);
                        }
                    }

                    Collider col = finalModel.GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                }
            }

            StartCoroutine(MessedUpPuffEffect());
            return;
        }

        if (finalDish != "") 
        {
            Debug.Log("✨ ประกอบร่างเสร็จ! กลายเป็น: " + finalDish);
            
            if (model1 != null) Destroy(model1);
            if (model2 != null) Destroy(model2);

            finalModel = SpawnModel(finalDish, finalDishSlot != null ? finalDishSlot : transform);
        }
    }

    IEnumerator MessedUpPuffEffect()
    {
        Transform target = finalDishSlot != null ? finalDishSlot : transform;
        Vector3 originalScale = target.localScale;
        
        // Comical squish bounce
        target.localScale = originalScale * 1.35f;
        yield return new WaitForSeconds(0.12f);
        target.localScale = originalScale * 0.85f;
        yield return new WaitForSeconds(0.1f);
        target.localScale = originalScale;
    }

    bool HasItems(string a, string b)
    {
        return (item1 == a && item2 == b) || (item1 == b && item2 == a);
    }

    GameObject SpawnModel(string itemName, Transform targetSlot)
    {
        if (all3DModelNames == null || all3DModels == null) return null;

        for (int i = 0; i < all3DModelNames.Length; i++)
        {
            if (all3DModelNames[i] == itemName && i < all3DModels.Length && all3DModels[i] != null)
            {
                GameObject newModel = Instantiate(all3DModels[i], targetSlot.position, targetSlot.rotation);
                newModel.transform.SetParent(targetSlot);
                
                Collider col = newModel.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                Rigidbody rb = newModel.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                return newModel;
            }
        }
        return null;
    }

    public void TakeFinalDish(PlayerInteraction player)
    {
        if (finalDish != "")
        {
            Sprite dishImage = player.GetFoodSprite(finalDish);
            player.PickUpCookedFood(finalDish, dishImage);
            
            ClearAllModels();
            item1 = "";
            item2 = "";
            finalDish = "";
        }
    }

    public void ClearAllModels()
    {
        if (model1 != null) Destroy(model1);
        if (model2 != null) Destroy(model2);
        if (finalModel != null) Destroy(finalModel);
    }
}