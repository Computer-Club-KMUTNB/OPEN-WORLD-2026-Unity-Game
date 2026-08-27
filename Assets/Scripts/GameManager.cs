using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int playerMoney = 0;
    public TextMeshProUGUI moneyTextUI; 

    void Start()
    {
        // อัปเดตข้อความบนจอตั้งแต่เริ่มเกม
        UpdateMoneyText();
    }

    public void ServeFood()
    {
        playerMoney += 50; 
        
        // พอได้เงินสั่งให้อัปเดต UI
        UpdateMoneyText(); 
        
        Debug.Log("Food served! Money: " + playerMoney + " Baht");
    }

    // เปลี่ยนข้อความบนจอ
    void UpdateMoneyText()
    {
        if (moneyTextUI != null)
        {
            moneyTextUI.text = "Money: " + playerMoney;
        }
    }
}