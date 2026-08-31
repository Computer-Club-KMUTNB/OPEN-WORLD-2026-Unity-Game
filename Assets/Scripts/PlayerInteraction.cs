using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 3f;
    
    [Header("Cooking System")]
    public Image cookingProgressBar; 
    private bool isCooking = false; 

    [Header("UI Hand System (2D)")]
    public string currentHeldItem = "";
    public Image handUI; 
    public Sprite[] allFoodSprites; 
    public string[] allFoodNames; 

    [Header("3D Hand System (ใหม่!)")]
    public Transform handPoint; // ตำแหน่งหน้ากล้องที่จะให้โมเดลมาลอยอยู่
    public GameObject[] all3DModels; // โมเดล 3D ของวัตถุดิบและอาหาร
    public string[] all3DModelNames; // ชื่อที่ตรงกับโมเดล (เช่น RawBeef, Steak)
    
    private GameObject currentHeldModel; // ตัวแปรจำว่าตอนนี้เสกโมเดลอะไรถือไว้อยู่

    void Update()
    {
        // โหมดสูตรโกง
        if (Input.GetKeyDown(KeyCode.Alpha1)) PickUpFood(0);

        if (isCooking) return;

        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // 1. ถ้ายิงเลเซอร์โดนลูกค้า
                if (hit.collider.CompareTag("Customer"))
                {
                    CustomerAI clickedCustomer = hit.collider.GetComponent<CustomerAI>();
                    if (clickedCustomer != null)
                    {
                        bool serveSuccess = clickedCustomer.ReceiveFood(currentHeldItem); 
                        if (serveSuccess) ClearHand(); 
                    }
                }
                // 2. ถ้ายิงเลเซอร์โดนกล่องวัตถุดิบดิบ
                else if (hit.collider.CompareTag("IngredientBox"))
                {
                    IngredientBox box = hit.collider.GetComponent<IngredientBox>();
                    if (box != null)
                    {
                        bool canTake = box.TryTakeIngredient(); 

                        if (canTake)
                        {
                            currentHeldItem = box.ingredientName;
                            Update3DHeldItem(currentHeldItem);
                        }
                        else
                        {
                            Debug.Log("ของหมดแล้ว หยิบไม่ได้!");
                        }
                    }
                }
                // 3. ถ้ายิงเลเซอร์โดนเตาทำอาหาร
                else if (hit.collider.CompareTag("CookingStation"))
                {
                    CookingStation station = hit.collider.GetComponent<CookingStation>();
                    if (station != null)
                    {
                        if (station.hasFinishedFood)
                        {
                            station.TakeFinishedFood(this);
                        }
                        else if (!station.isCooking && currentHeldItem == station.requiredIngredient)
                        {
                            ClearHand(); // เอาของลงเตามือต้องว่าง
                            station.StartCooking(); 
                            StartCoroutine(CookRoutine(station.cookTime)); 
                        }
                    }
                }
                // 4. ถ้ายิงเลเซอร์โดนโต๊ะจัดจาน
                else if (hit.collider.CompareTag("PlateStation"))
                {
                    PlateStation plate = hit.collider.GetComponent<PlateStation>();
                    if (plate != null)
                    {
                        if (plate.finalDish != "" && currentHeldItem == "")
                        {
                            plate.TakeFinalDish(this);
                        }
                        else if (currentHeldItem != "" && plate.finalDish == "")
                        {
                            plate.AddIngredient(currentHeldItem, this);
                        }
                    }
                }
                else if (hit.collider.CompareTag("Computer"))
                {
                    ComputerTerminal computer = hit.collider.GetComponent<ComputerTerminal>();
                    if (computer != null)
                    {
                        computer.OrderSupplies(); // สั่งของเข้าสต็อคทันทีที่คลิก
                    }
                }
                else if (hit.collider.CompareTag("ShopBell"))
                {
                    ShopBell bell = hit.collider.GetComponentInParent<ShopBell>();
                    if (bell != null)
                    {
                        bell.RingBell(); // สั่งให้กระดิ่งดังและสลับสถานะร้าน
                    }
                }
            }
        }
    }

    // เอาไว้เสกโมเดล 3D ขึ้นมาใส่มือ
    void Update3DHeldItem(string itemName)
    {
        // 1. ซ่อน UI รูปภาพ 2D ไปก่อน
        if (handUI != null) handUI.gameObject.SetActive(false);

        // 2. ลบโมเดลเก่าทิ้ง
        if (currentHeldModel != null) Destroy(currentHeldModel);

        // 3. วนลูปหาโมเดล 3D ที่ชื่อตรงกับของที่เพิ่งหยิบ
        for (int i = 0; i < all3DModelNames.Length; i++)
        {
            if (all3DModelNames[i] == itemName && all3DModels[i] != null)
            {
                // เสกโมเดลขึ้นมาที่ตำแหน่ง HandPoint
                currentHeldModel = Instantiate(all3DModels[i], handPoint.position, handPoint.rotation);
                
                // สั่งให้เป็นลูกของ HandPoint เพื่อให้มันขยับหันซ้ายขวาตามกล้องผู้เล่น
                currentHeldModel.transform.SetParent(handPoint); 
                
                // ปิดระบบฟิสิกส์ (กันไม่ให้เนื้อหมูกระเด็นชนหน้ากล้อง)
                Collider col = currentHeldModel.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                
                Rigidbody rb = currentHeldModel.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                break; // เจอแล้วหยุดหา
            }
        }
    }
    // ----------------------------------------------------

    public void PickUpCookedFood(string foodName, Sprite foodSprite)
    {
        currentHeldItem = foodName;
        Update3DHeldItem(currentHeldItem); // เรียกให้โชว์โมเดล 3D 
        
        // ถ้าจานอาหารสุกยังไม่มีโมเดล 3D โชว์รูป 2D แทน (ระบบสำรอง)
        if (currentHeldModel == null && handUI != null)
        {
            handUI.sprite = foodSprite;
            handUI.gameObject.SetActive(true); 
        }
    }

    public void ClearHand()
    {
        currentHeldItem = "";
        if (handUI != null) handUI.gameObject.SetActive(false);
        if (currentHeldModel != null) Destroy(currentHeldModel); // ลบโมเดล 3D ในมือทิ้ง
    }

    public Sprite GetFoodSprite(string foodName)
    {
        for (int i = 0; i < allFoodNames.Length; i++)
        {
            if (allFoodNames[i] == foodName) return allFoodSprites[i];
        }
        return null;
    }

    // ฟังก์ชันสูตรโกง (อันเก่า)
    void PickUpFood(int menuIndex)
    {
        currentHeldItem = allFoodNames[menuIndex];
        Update3DHeldItem(currentHeldItem);
    }

    IEnumerator CookRoutine(float cookTime)
    {
        isCooking = true;
        float timer = 0f;
        while (timer < cookTime)
        {
            timer += Time.deltaTime; 
            if (cookingProgressBar != null) cookingProgressBar.fillAmount = timer / cookTime; 
            yield return null; 
        }
        if (cookingProgressBar != null) cookingProgressBar.fillAmount = 0f; 
        isCooking = false; 
    }
}