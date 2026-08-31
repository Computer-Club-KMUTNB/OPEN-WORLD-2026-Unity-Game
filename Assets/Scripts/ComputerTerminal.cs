using UnityEngine;

public class ComputerTerminal : MonoBehaviour
{
    [Header("ตั้งค่าการสั่งซื้อ")]
    public int orderAmount = 5; // จำนวนของที่ได้รับ
    public int orderCost = 100; // ราคาค่าสั่งซื้อ

    public void OrderSupplies()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            // เช็คและหักเงินก่อนเติมของ
            if (gm.DeductMoney(orderCost))
            {
                gm.rawRiceStock += orderAmount;
                gm.rawVeggieStock += orderAmount;
                SaveSystem.Save();
                Debug.Log("สั่งซื้อสำเร็จ จ่ายเงิน " + orderCost + " บาท | ข้าว=" + gm.rawRiceStock + " | ผัก=" + gm.rawVeggieStock);
            }
            else
            {
                Debug.Log("เงินไม่พอ ต้องการ " + orderCost + " บาท (ตอนนี้คุณมีเงิน " + gm.playerMoney + " บาท)");
            }
        }
    }
}