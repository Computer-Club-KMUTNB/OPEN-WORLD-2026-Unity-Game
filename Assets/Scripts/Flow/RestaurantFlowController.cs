using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestaurantFlowController : MonoBehaviour
{
    public static RestaurantFlowController Instance { get; private set; }

    [Header("Dungeon Portal Settings")]
    public string dungeonDoorObjectName = "DoortoJRscenc";
    public float doorInteractionDistance = 2.0f;
    public string dungeonSceneName = "Dev_Dungeon_Flow";

    [Header("Shift Summary & Pause")]
    public string shiftSummarySceneName = "ShiftSummary_Cute";
    public string pauseSceneName = "PauseMenu_Cute";

    [Header("Live Shift Stats")]
    public int servedOrders = 0;
    public int happyGuests = 0;
    public int totalGuests = 0;
    public int dishesCooked = 0;
    public float shiftTimer = 0f;

    [Header("Shift Status")]
    public bool isShiftActive = false;

    private Transform playerTransform;
    private Camera playerCam;
    private GameObject dungeonDoorObject;
    private GameObject bellObject;
    private GameObject computerObject;
    private IngredientBox[] ingredientBoxes;
    private CookingStation[] cookingStations;
    private PlateStation[] plateStations;

    private bool isNearDungeonDoor = false;
    private int startingMoney = 0;
    private CustomerSpawner customerSpawner;
    private DayTimer dayTimer;
    private Light doorGlowLight;
    private Texture2D whiteTexture;

    private string currentHoverText = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            startingMoney = GameManager.Instance.playerMoney;
        }

        customerSpawner = FindAnyObjectByType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.isShiftActive = false;
        }

        dayTimer = FindAnyObjectByType<DayTimer>();
        FindInteractiveObjects();
        SetupDoorGlow();
    }

    private void Update()
    {
        // 1. Sync shift state with DayTimer
        if (dayTimer != null)
        {
            if (dayTimer.isShopOpen != isShiftActive)
            {
                isShiftActive = dayTimer.isShopOpen;
                if (isShiftActive)
                {
                    OnShiftStarted();
                }
            }
        }

        if (isShiftActive)
        {
            shiftTimer += Time.deltaTime;
        }

        if (playerTransform == null || dungeonDoorObject == null || bellObject == null)
        {
            FindInteractiveObjects();
            SetupDoorGlow();
        }

        CheckDoorProximity();
        UpdateDoorGlowEffect();
        UpdateRaycastHoverText();

        // 2. Door interaction
        if (isNearDungeonDoor && (Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonDown(0) && IsLookingAtDoor())))
        {
            if (!isShiftActive)
            {
                EmbarkToDungeon();
            }
        }

        // 3. Pause menu toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void OnShiftStarted()
    {
        shiftTimer = 0f;
        servedOrders = 0;
        happyGuests = 0;
        totalGuests = 0;
        dishesCooked = 0;

        if (GameManager.Instance != null)
        {
            startingMoney = GameManager.Instance.playerMoney;
        }

        if (customerSpawner != null) customerSpawner.StartShift();
        Debug.Log("🍽️ [RestaurantFlow] Restaurant opened via Service Bell!");
    }

    public void StartShift()
    {
        if (dayTimer != null && !dayTimer.isShopOpen)
        {
            dayTimer.OpenShop();
        }
        else
        {
            isShiftActive = true;
            OnShiftStarted();
        }
    }

    public void EndRestaurantShift()
    {
        isShiftActive = false;
        Debug.Log("🍰 Closing restaurant shift & calculating summary...");

        if (customerSpawner != null)
        {
            customerSpawner.StopShift();
        }

        int currentMoney = (GameManager.Instance != null) ? GameManager.Instance.playerMoney : 0;
        int revenue = Mathf.Max(servedOrders * 50, currentMoney - startingMoney);
        int upkeep = Mathf.Max(30, dishesCooked * 10);
        int tips = Mathf.Max(0, happyGuests * 20);

        // --- Realistic Multi-Ratio Star Rating Calculation ---
        float rating = 1.0f;
        if (servedOrders > 0)
        {
            float totalActualGuests = Mathf.Max(totalGuests, servedOrders);
            float serveRatio = Mathf.Clamp01((float)servedOrders / totalActualGuests);
            float happyRatio = Mathf.Clamp01((float)happyGuests / totalActualGuests);
            float dishRatio = Mathf.Clamp01((float)dishesCooked / Mathf.Max(1, servedOrders));

            float score = (serveRatio * 2.5f) + (happyRatio * 1.75f) + (dishRatio * 0.75f);
            if (servedOrders >= 5 && serveRatio >= 0.9f) score += 0.5f;

            rating = Mathf.Clamp(score, 1.0f, 5.0f);
        }
        else
        {
            rating = 1.0f;
        }

        SummaryDataBridge.RecordShiftSession(
            happy: Mathf.Max(happyGuests, servedOrders),
            total: Mathf.Max(totalGuests, servedOrders),
            dishes: dishesCooked,
            revenue: revenue,
            upkeep: upkeep,
            tips: tips,
            rating: rating
        );

        string target = shiftSummarySceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "ShiftSummary_Cute";
        }
        SceneManager.LoadScene(target);
    }

    private void SetupDoorGlow()
    {
        if (dungeonDoorObject == null) return;

        doorGlowLight = dungeonDoorObject.GetComponentInChildren<Light>();
        if (doorGlowLight == null)
        {
            GameObject lightObj = new GameObject("DungeonDoorGlow");
            lightObj.transform.SetParent(dungeonDoorObject.transform);
            lightObj.transform.localPosition = new Vector3(0, 1.2f, 0);

            doorGlowLight = lightObj.AddComponent<Light>();
            doorGlowLight.type = LightType.Point;
            doorGlowLight.color = new Color(1.0f, 0.35f, 0.2f);
            doorGlowLight.range = 7f;
            doorGlowLight.intensity = 3.5f;
        }
    }

    private void UpdateDoorGlowEffect()
    {
        if (doorGlowLight != null)
        {
            if (!isShiftActive)
            {
                doorGlowLight.enabled = true;
                float pulse = 2.0f + Mathf.PingPong(Time.time * 3f, 2.5f);
                doorGlowLight.intensity = pulse;
            }
            else
            {
                doorGlowLight.enabled = false;
            }
        }
    }

    private void FindInteractiveObjects()
    {
        if (playerTransform == null)
        {
            FirstPersonController fpc = FindAnyObjectByType<FirstPersonController>();
            if (fpc != null)
            {
                playerTransform = fpc.transform;
                playerCam = fpc.playerCamera != null ? fpc.playerCamera.GetComponent<Camera>() : fpc.GetComponentInChildren<Camera>();
            }

            if (playerTransform == null)
            {
                PlayerInteraction pi = FindAnyObjectByType<PlayerInteraction>();
                if (pi != null)
                {
                    playerTransform = pi.transform;
                    playerCam = pi.playerCamera;
                }
            }

            if (playerCam == null) playerCam = Camera.main;
            if (playerTransform == null && playerCam != null) playerTransform = playerCam.transform;
        }

        if (dungeonDoorObject == null)
        {
            dungeonDoorObject = GameObject.Find("DoortoJRscenc") ?? GameObject.Find("DoortoJR") ?? GameObject.Find(dungeonDoorObjectName);
            if (dungeonDoorObject == null)
            {
                GameObject changeScene = GameObject.Find("ChangeScene");
                if (changeScene != null)
                {
                    Transform t = changeScene.transform.Find("DoortoJRscenc") ?? changeScene.transform.Find("DoortoJR");
                    if (t != null) dungeonDoorObject = t.gameObject;
                    else dungeonDoorObject = changeScene;
                }
            }
        }

        if (bellObject == null)
        {
            ShopBell bell = FindAnyObjectByType<ShopBell>();
            if (bell != null) bellObject = bell.gameObject;
            if (bellObject == null) bellObject = GameObject.Find("ServiceBell");
        }

        if (computerObject == null)
        {
            ComputerTerminal comp = FindAnyObjectByType<ComputerTerminal>();
            if (comp != null) computerObject = comp.gameObject;
            if (computerObject == null) computerObject = GameObject.Find("Computer");
        }

        ingredientBoxes = FindObjectsByType<IngredientBox>(FindObjectsSortMode.None);
        cookingStations = FindObjectsByType<CookingStation>(FindObjectsSortMode.None);
        plateStations = FindObjectsByType<PlateStation>(FindObjectsSortMode.None);
    }

    private void CheckDoorProximity()
    {
        if (playerTransform == null || dungeonDoorObject == null)
        {
            isNearDungeonDoor = false;
            return;
        }

        float dist = Vector3.Distance(playerTransform.position, dungeonDoorObject.transform.position);
        isNearDungeonDoor = dist <= doorInteractionDistance;
    }

    private bool IsLookingAtDoor()
    {
        if (playerCam == null || dungeonDoorObject == null) return false;
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, doorInteractionDistance + 1.5f))
        {
            if (hit.collider.gameObject == dungeonDoorObject || hit.transform.IsChildOf(dungeonDoorObject.transform) || dungeonDoorObject.transform.IsChildOf(hit.transform))
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateRaycastHoverText()
    {
        currentHoverText = "";
        if (playerCam == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3.8f))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hit.collider.GetComponentInParent<ShopBell>() != null || hitObj.name.ToLower().Contains("bell"))
            {
                currentHoverText = isShiftActive 
                    ? "🛎️ [Click / E] Ring Bell to End Shift" 
                    : "🛎️ [Click / E] Ring Bell to Open Shop";
                return;
            }

            if (hit.collider.GetComponentInParent<ComputerTerminal>() != null || hitObj.name.ToLower().Contains("computer"))
            {
                currentHoverText = "💻 [Click / E] Buy Rice & Veggies (100 G)";
                return;
            }

            if (hitObj == dungeonDoorObject || hit.transform.IsChildOf(dungeonDoorObject.transform) || hitObj.name.Contains("DoortoJR"))
            {
                currentHoverText = isShiftActive 
                    ? "🔒 [Locked] Close shop before hunting" 
                    : "🥩 [Click / E] Go Hunting! (Get Beef & Pork)";
                return;
            }

            IngredientBox box = hit.collider.GetComponentInParent<IngredientBox>();
            if (box != null)
            {
                int stock = 0;
                if (GameManager.Instance != null)
                {
                    if (box.ingredientName == "RawBeef") stock = GameManager.globalBeef;
                    else if (box.ingredientName == "RawPork") stock = GameManager.globalPork;
                    else if (box.ingredientName == "RawRice") stock = GameManager.globalRice;
                    else if (box.ingredientName == "RawVeggie") stock = GameManager.globalVeggie;
                }
                currentHoverText = $"📦 [Click / E] Take {box.ingredientName} ({stock})";
                return;
            }

            CookingStation station = hit.collider.GetComponentInParent<CookingStation>();
            if (station != null)
            {
                if (station.hasFinishedFood)
                {
                    currentHoverText = $"✨ [Click / E] Take {station.resultFoodName}";
                }
                else if (station.isCooking)
                {
                    currentHoverText = "🍳 Cooking...";
                }
                else
                {
                    currentHoverText = $"🔥 [Click / E] Cook {station.requiredIngredient}";
                }
                return;
            }

            PlateStation plate = hit.collider.GetComponentInParent<PlateStation>();
            if (plate != null)
            {
                if (plate.finalDish != "")
                {
                    currentHoverText = $"🍽️ [Click / E] Pick up {plate.finalDish}";
                }
                else
                {
                    currentHoverText = "🍽️ [Click / E] Assemble Plate";
                }
                return;
            }

            CustomerAI customer = hit.collider.GetComponentInParent<CustomerAI>();
            if (customer != null)
            {
                currentHoverText = "🐱 [Click / E] Serve Customer";
                return;
            }
        }
    }

    public void RegisterDishCooked() => dishesCooked++;
    public void RegisterCustomerServed(bool isHappy = true)
    {
        servedOrders++;
        totalGuests++;
        if (isHappy) happyGuests++;
    }

    public void EmbarkToDungeon()
    {
        Debug.Log("⚔️ Embarking to Dungeon Hunt...");
        string target = dungeonSceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "DemoScene";
        }
        SceneManager.LoadScene(target);
    }

    private void TogglePauseMenu()
    {
        Scene pauseScene = SceneManager.GetSceneByName(pauseSceneName);
        if (pauseScene.isLoaded)
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.UnloadSceneAsync(pauseSceneName);
        }
        else
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
        }
    }

    private void DrawBox(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = old;
    }

    private void DrawWorldPrompt(Vector3 worldPos, string text, Color color, float width = 300, float height = 34)
    {
        if (playerCam == null) return;
        Vector3 screenPos = playerCam.WorldToScreenPoint(worldPos);
        if (screenPos.z > 0 && screenPos.z < 12f)
        {
            float x = screenPos.x - width / 2f;
            float y = Screen.height - screenPos.y - height / 2f;

            DrawBox(new Rect(x, y, width, height), new Color(0.08f, 0.08f, 0.12f, 0.92f));

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = color;
            GUI.Label(new Rect(x, y + 4, width, height - 8), text, style);
        }
    }

    private void OnGUI()
    {
        // -------------------------------------------------------------
        // PREVENT HUD DRAWING OVER PAUSE MENU (FREEZE OVERLAY FIX)
        // -------------------------------------------------------------
        if (Time.timeScale <= 0f) return;
        Scene pauseCute = SceneManager.GetSceneByName(pauseSceneName);
        if (pauseCute.isLoaded) return;

        // -------------------------------------------------------------
        // 1. LARGER & CLEARER 3D WORLD FLOATING PROMPTS
        // -------------------------------------------------------------
        if (bellObject != null)
        {
            string bellMsg = isShiftActive ? "🛎️ BELL: [Click / E] Close Shift" : "🛎️ BELL: [Click / E] Open Shop";
            DrawWorldPrompt(bellObject.transform.position + Vector3.up * 0.55f, bellMsg, new Color(1f, 0.85f, 0.3f), 300, 34);
        }

        if (computerObject != null)
        {
            DrawWorldPrompt(computerObject.transform.position + Vector3.up * 0.75f, "💻 COMPUTER: [Click / E] Buy Rice & Veg (100G)", new Color(0.45f, 0.85f, 1f), 340, 34);
        }

        if (dungeonDoorObject != null)
        {
            if (!isShiftActive)
            {
                DrawWorldPrompt(dungeonDoorObject.transform.position + Vector3.up * 2.3f, "🥩 DUNGEON HUNT: [Press E] Get Beef & Pork!", new Color(1f, 0.45f, 0.4f), 360, 36);
            }
            else
            {
                DrawWorldPrompt(dungeonDoorObject.transform.position + Vector3.up * 2.3f, "🔒 DOOR LOCKED (In Shift)", new Color(0.7f, 0.7f, 0.7f), 260, 32);
            }
        }

        // -------------------------------------------------------------
        // 2. TOP-CENTER CUTE STATUS BANNER & PASTEL TIME BAR (COMPACT & FITTED)
        // -------------------------------------------------------------
        float bannerW = 440;
        float bannerH = isShiftActive ? 62 : 38;
        float bannerX = (Screen.width - bannerW) / 2f;
        float bannerY = 16;

        DrawBox(new Rect(bannerX, bannerY, bannerW, bannerH), new Color(0.12f, 0.12f, 0.16f, 0.92f));

        GUIStyle statusHeader = new GUIStyle(GUI.skin.label);
        statusHeader.fontSize = 14;
        statusHeader.fontStyle = FontStyle.Bold;
        statusHeader.alignment = TextAnchor.MiddleCenter;

        GUIStyle subLabel = new GUIStyle(GUI.skin.label);
        subLabel.fontSize = 11;
        subLabel.fontStyle = FontStyle.Normal;
        subLabel.alignment = TextAnchor.MiddleCenter;
        subLabel.normal.textColor = new Color(0.9f, 0.9f, 0.95f);

        if (!isShiftActive)
        {
            statusHeader.normal.textColor = new Color(0.45f, 0.95f, 0.6f);
            GUI.Label(new Rect(bannerX, bannerY + 8, bannerW, 22), "🍵 PREPARATION MODE  •  🛎️ Ring Bell to Open Shop!", statusHeader);
        }
        else
        {
            statusHeader.normal.textColor = new Color(1f, 0.82f, 0.28f);
            float maxTime = (dayTimer != null && dayTimer.dayDuration > 0) ? dayTimer.dayDuration : 180f;
            float currentRemaining = (dayTimer != null) ? dayTimer.timeRemaining : Mathf.Max(0, maxTime - shiftTimer);
            float progress = Mathf.Clamp01(currentRemaining / maxTime);

            int remMin = Mathf.FloorToInt(currentRemaining / 60f);
            int remSec = Mathf.FloorToInt(currentRemaining % 60f);

            GUI.Label(new Rect(bannerX, bannerY + 4, bannerW, 18), $"🍽️ SHOP OPEN  •  ⏱️ {remMin:D2}:{remSec:D2}  •  Served: {servedOrders}", statusHeader);

            // Cute Pastel Progress Bar
            float barW = bannerW - 32;
            float barH = 8;
            float barX = bannerX + 16;
            float barY = bannerY + 25;

            DrawBox(new Rect(barX, barY, barW, barH), new Color(0.25f, 0.25f, 0.32f, 0.95f));

            Color barColor = progress > 0.45f ? new Color(0.4f, 0.92f, 0.55f) : (progress > 0.2f ? new Color(1f, 0.85f, 0.3f) : new Color(1f, 0.45f, 0.45f));
            DrawBox(new Rect(barX, barY, barW * progress, barH), barColor);

            GUI.Label(new Rect(bannerX, bannerY + 38, bannerW, 18), "🛎️ Click Bell on counter to End Shift", subLabel);
        }

        // -------------------------------------------------------------
        // 3. TOP-LEFT CUTE PANTRY & MONEY HUD (CLEAN & FIT)
        // -------------------------------------------------------------
        float pantryW = 420;
        float pantryH = 38;
        DrawBox(new Rect(16, 16, pantryW, pantryH), new Color(0.12f, 0.12f, 0.16f, 0.92f));

        GUIStyle stockStyle = new GUIStyle(GUI.skin.label);
        stockStyle.fontSize = 12;
        stockStyle.fontStyle = FontStyle.Bold;
        stockStyle.alignment = TextAnchor.MiddleCenter;
        stockStyle.normal.textColor = Color.white;

        int beef = GameManager.globalBeef;
        int pork = GameManager.globalPork;
        int rice = GameManager.globalRice;
        int veggie = GameManager.globalVeggie;
        int money = GameManager.globalMoney;

        GUI.Label(new Rect(16, 24, pantryW, 20), $"🥩 {beef}   🥓 {pork}   🥦 {veggie}   🍚 {rice}   |   💰 <color=#FFD700>{money} G</color>", stockStyle);

        // -------------------------------------------------------------
        // 4. BOTTOM-LEFT ON-SCREEN COOKING & SUPPLIES GUIDE (RESTORED)
        // -------------------------------------------------------------
        float guideW = 560;
        float guideH = 88;
        float guideX = 16;
        float guideY = Screen.height - guideH - 16;

        DrawBox(new Rect(guideX, guideY, guideW, guideH), new Color(0.1f, 0.1f, 0.14f, 0.92f));

        GUIStyle guideTitle = new GUIStyle(GUI.skin.label);
        guideTitle.fontSize = 13;
        guideTitle.fontStyle = FontStyle.Bold;
        guideTitle.normal.textColor = new Color(0.95f, 0.85f, 0.4f);

        GUIStyle guideBody = new GUIStyle(GUI.skin.label);
        guideBody.fontSize = 11;
        guideBody.fontStyle = FontStyle.Normal;
        guideBody.normal.textColor = new Color(0.9f, 0.9f, 0.95f);

        GUI.Label(new Rect(guideX + 12, guideY + 6, guideW - 24, 18), "📖 RESTAURANT & COOKING GUIDE", guideTitle);
        GUI.Label(new Rect(guideX + 12, guideY + 25, guideW - 24, 16), "🍳 Cooking: 1. Click Box (Take) ➔ 2. Stove (Cook) ➔ 3. Take Cooked ➔ 4. Plate Table ➔ 5. Serve!", guideBody);
        GUI.Label(new Rect(guideX + 12, guideY + 44, guideW - 24, 16), "💻 Supplies: Click Computer Terminal to Buy 🥦 Veggie (+5) & 🍚 Rice (+5) for 100 Gold", guideBody);
        GUI.Label(new Rect(guideX + 12, guideY + 63, guideW - 24, 16), "🏹 Raw Meats: Enter Glowing Red Door in Prep Mode to hunting for Beef & Pork!", guideBody);

        // -------------------------------------------------------------
        // 5. CENTER CROSSHAIR HOVER PROMPT
        // -------------------------------------------------------------
        if (!string.IsNullOrEmpty(currentHoverText))
        {
            float hoverW = 420;
            float hoverH = 30;
            float hoverX = (Screen.width - hoverW) / 2f;
            float hoverY = (Screen.height / 2f) + 36;

            DrawBox(new Rect(hoverX, hoverY, hoverW, hoverH), new Color(0.08f, 0.08f, 0.12f, 0.9f));

            GUIStyle hoverStyle = new GUIStyle(GUI.skin.label);
            hoverStyle.fontSize = 12;
            hoverStyle.fontStyle = FontStyle.Bold;
            hoverStyle.alignment = TextAnchor.MiddleCenter;
            hoverStyle.normal.textColor = new Color(1f, 0.92f, 0.45f);

            GUI.Label(new Rect(hoverX, hoverY + 5, hoverW, 20), currentHoverText, hoverStyle);
        }
        else if (isNearDungeonDoor)
        {
            float doorPromptW = 380;
            float doorPromptH = 34;
            float doorPromptX = (Screen.width - doorPromptW) / 2f;
            float doorPromptY = Screen.height - 110;

            DrawBox(new Rect(doorPromptX, doorPromptY, doorPromptW, doorPromptH), new Color(0.12f, 0.1f, 0.15f, 0.92f));

            GUIStyle doorStyle = new GUIStyle(GUI.skin.label);
            doorStyle.fontSize = 13;
            doorStyle.fontStyle = FontStyle.Bold;
            doorStyle.alignment = TextAnchor.MiddleCenter;

            if (!isShiftActive)
            {
                doorStyle.normal.textColor = new Color(1.0f, 0.45f, 0.45f);
                GUI.Label(new Rect(doorPromptX, doorPromptY + 7, doorPromptW, 20), "🥩 Press [E] to Get Beef & Pork", doorStyle);
            }
            else
            {
                doorStyle.normal.textColor = new Color(1.0f, 0.7f, 0.3f);
                GUI.Label(new Rect(doorPromptX, doorPromptY + 7, doorPromptW, 20), "🔒 Door locked during shift", doorStyle);
            }
        }
    }
}
