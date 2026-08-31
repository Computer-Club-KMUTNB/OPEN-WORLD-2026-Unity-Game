using UnityEngine;
using TMPro; // สำคัญมาก: ต้องใส่เพื่อใช้งาน TextMeshPro

public class IngredientBox : MonoBehaviour
{
    public string ingredientName; // พิมพ์ชื่อให้ตรงกับ GameManager เช่น RawBeef, RawPork, RawRice, RawVeggie
    public TextMeshPro stockTextUI; // ช่องใส่ข้อความบนกล่อง

    void Start()
    {
        UpdateDisplay();
    }

    void Update()
    {
        // คอยอัปเดตตัวเลขให้ตรงกับ GameManager ตลอดเวลา (รวมถึงตอนสั่งซื้อจากคอมด้วย)
        UpdateDisplay();
    }

    // ฟังก์ชันอัปเดตข้อความบนกล่อง
    public void UpdateDisplay()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || stockTextUI == null) return;

        int currentStock = 0;

        // ดึงค่าสต็อคตามชนิดวัตถุดิบ
        if (ingredientName == "RawBeef") currentStock = gm.rawBeefStock;
        else if (ingredientName == "RawPork") currentStock = gm.rawPorkStock;
        else if (ingredientName == "RawRice") currentStock = gm.rawRiceStock;
        else if (ingredientName == "RawVeggie") currentStock = gm.rawVeggieStock;

        // เปลี่ยนข้อความบนกล่อง
        stockTextUI.text = ingredientName + "\n" + currentStock;

        // เปลี่ยนข้อความตัวหนังสือเป็นสีแดง
        if (currentStock <= 0)
        {
            stockTextUI.color = Color.red;
            stockTextUI.text = ingredientName + "\n[Out of Stock]";
        }
        else
        {
            stockTextUI.color = Color.white;
        }
    }

    // ฟังก์ชันหยิบวัตถุดิบ
    public bool TryTakeIngredient()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return false;

        bool success = false;

        if (ingredientName == "RawBeef" && gm.rawBeefStock > 0)
        {
            gm.rawBeefStock--;
            success = true;
        }
        else if (ingredientName == "RawPork" && gm.rawPorkStock > 0)
        {
            gm.rawPorkStock--;
            success = true;
        }
        else if (ingredientName == "RawRice" && gm.rawRiceStock > 0)
        {
            gm.rawRiceStock--;
            success = true;
        }
        else if (ingredientName == "RawVeggie" && gm.rawVeggieStock > 0)
        {
            gm.rawVeggieStock--;
            success = true;
        }

        if (success)
        {
            UpdateDisplay();
            SaveSystem.Save();
            return true;
        }
        
        return false;
    }
}