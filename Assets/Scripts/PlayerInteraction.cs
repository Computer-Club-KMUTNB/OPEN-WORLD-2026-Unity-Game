using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance { get; private set; }

    public Camera playerCamera;
    public float interactDistance = 3.5f;
    
    [Header("Cooking System")]
    public Image cookingProgressBar; 
    private bool isCooking = false; 

    [Header("UI Hand System (2D)")]
    public string currentHeldItem = "";
    public Image handUI; 
    public Sprite[] allFoodSprites; 
    public string[] allFoodNames; 

    [Header("3D Hand System")]
    public Transform handPoint; // ตำแหน่งหน้ากล้องที่จะให้โมเดลมาลอยอยู่
    public GameObject[] all3DModels; // โมเดล 3D ของวัตถุดิบและอาหาร
    public string[] all3DModelNames; // ชื่อที่ตรงกับโมเดล (เช่น RawBeef, Steak)
    
    private GameObject currentHeldModel; // ตัวแปรจำว่าตอนนี้เสกโมเดลอะไรถือไว้อยู่

    private void Awake()
    {
        Instance = this;
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null) playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // โหมดสูตรโกง
        if (Input.GetKeyDown(KeyCode.Alpha1) && allFoodNames != null && allFoodNames.Length > 0) PickUpFood(0);

        if (isCooking) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) 
        {
            if (playerCamera == null) playerCamera = Camera.main;
            if (playerCamera == null) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // 1. ลูกค้า
                CustomerAI clickedCustomer = hit.collider.GetComponentInParent<CustomerAI>();
                if (clickedCustomer != null)
                {
                    bool serveSuccess = clickedCustomer.ReceiveFood(currentHeldItem); 
                    if (serveSuccess) ClearHand(); 
                    return;
                }

                // 2. กล่องวัตถุดิบ
                IngredientBox box = hit.collider.GetComponentInParent<IngredientBox>();
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
                    return;
                }

                // 3. เตาทำอาหาร
                CookingStation station = hit.collider.GetComponentInParent<CookingStation>();
                if (station != null)
                {
                    if (station.hasFinishedFood)
                    {
                        station.TakeFinishedFood(this);
                    }
                    else if (!station.isCooking && currentHeldItem == station.requiredIngredient)
                    {
                        ClearHand();
                        station.StartCooking(); 
                        StartCoroutine(CookRoutine(station.cookTime)); 
                    }
                    return;
                }

                // 4. โต๊ะจัดจาน
                PlateStation plate = hit.collider.GetComponentInParent<PlateStation>();
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
                    return;
                }

                // 5. คอมพิวเตอร์สั่งซื้อของ
                ComputerTerminal computer = hit.collider.GetComponentInParent<ComputerTerminal>();
                if (computer != null || hit.collider.CompareTag("Computer") || hit.collider.name.ToLower().Contains("computer"))
                {
                    if (computer == null) computer = hit.collider.GetComponent<ComputerTerminal>() ?? hit.collider.GetComponentInParent<ComputerTerminal>();
                    if (computer != null)
                    {
                        computer.OrderSupplies();
                        return;
                    }
                }

                // 6. กระดิ่งเปิด/ปิดร้าน
                ShopBell bell = hit.collider.GetComponentInParent<ShopBell>();
                if (bell != null || hit.collider.CompareTag("ShopBell") || hit.collider.name.ToLower().Contains("bell"))
                {
                    if (bell == null) bell = hit.collider.GetComponent<ShopBell>() ?? hit.collider.GetComponentInParent<ShopBell>();
                    if (bell != null)
                    {
                        bell.RingBell();
                        return;
                    }
                }
            }
        }
    }

    // เอาไว้เสกโมเดล 3D ขึ้นมาใส่มือ
    public void Update3DHeldItem(string itemName)
    {
        // 1. ซ่อน UI รูปภาพ 2D ไปก่อน
        if (handUI != null) handUI.gameObject.SetActive(false);

        // 2. ลบโมเดลเก่าทิ้ง
        if (currentHeldModel != null) Destroy(currentHeldModel);

        if (string.IsNullOrEmpty(itemName) || handPoint == null || all3DModelNames == null) return;

        // 3. วนลูปหาโมเดล 3D ที่ชื่อตรงกับของที่เพิ่งหยิบ
        for (int i = 0; i < all3DModelNames.Length; i++)
        {
            if (all3DModelNames[i] == itemName && all3DModels != null && i < all3DModels.Length && all3DModels[i] != null)
            {
                currentHeldModel = Instantiate(all3DModels[i], handPoint.position, handPoint.rotation);
                currentHeldModel.transform.SetParent(handPoint); 
                
                Collider col = currentHeldModel.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                
                Rigidbody rb = currentHeldModel.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                break;
            }
        }
    }

    public void PickUpCookedFood(string foodName, Sprite foodSprite)
    {
        currentHeldItem = foodName;
        Update3DHeldItem(currentHeldItem); 
        
        if (currentHeldModel == null && handUI != null && foodSprite != null)
        {
            handUI.sprite = foodSprite;
            handUI.gameObject.SetActive(true); 
        }
    }

    public void ClearHand()
    {
        currentHeldItem = "";
        if (handUI != null) handUI.gameObject.SetActive(false);
        if (currentHeldModel != null) Destroy(currentHeldModel);
    }

    public Sprite GetFoodSprite(string foodName)
    {
        if (allFoodNames == null || allFoodSprites == null) return null;
        for (int i = 0; i < allFoodNames.Length; i++)
        {
            if (allFoodNames[i] == foodName && i < allFoodSprites.Length) return allFoodSprites[i];
        }
        return null;
    }

    void PickUpFood(int menuIndex)
    {
        if (allFoodNames != null && menuIndex < allFoodNames.Length)
        {
            currentHeldItem = allFoodNames[menuIndex];
            Update3DHeldItem(currentHeldItem);
        }
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