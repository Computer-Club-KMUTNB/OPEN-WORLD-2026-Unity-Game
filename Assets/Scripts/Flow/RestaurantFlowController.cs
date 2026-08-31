using UnityEngine;
using UnityEngine.SceneManagement;

public class RestaurantFlowController : MonoBehaviour
{
    public static RestaurantFlowController Instance { get; private set; }

    [Header("Dungeon Portal Settings")]
    [Tooltip("ชื่อ GameObject ประตูไปดันเจี้ยน")]
    public string dungeonDoorObjectName = "DoortoJRscenc";
    public float doorInteractionDistance = 4.5f;
    public string dungeonSceneName = "Dev_Dungeon_Flow";

    [Header("Shift Settings")]
    public string shiftSummarySceneName = "ShiftSummary_Cute";
    public KeyCode shiftToggleKey = KeyCode.C;
    public bool isShiftActive = false;

    [Header("Pause Menu")]
    public string pauseSceneName = "PauseMenu_Cute";

    [Header("Live Shift Stats")]
    public int servedOrders = 0;
    public int happyGuests = 0;
    public int totalGuests = 0;
    public int dishesCooked = 0;
    public float shiftTimer = 0f;

    private Transform playerTransform;
    private GameObject dungeonDoorObject;
    private bool isNearDungeonDoor = false;
    private int startingMoney = 0;
    private CustomerSpawner customerSpawner;

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
            customerSpawner.isShiftActive = false; // เริ่มต้นในโหมดเตรียมร้าน (Preparation Phase)
        }

        FindPlayerAndDoor();
    }

    private void Update()
    {
        if (isShiftActive)
        {
            shiftTimer += Time.deltaTime;
        }

        if (playerTransform == null || dungeonDoorObject == null)
        {
            FindPlayerAndDoor();
        }

        CheckDoorProximity();

        // 1. กด E หรือคลิกซ้ายเมื่ออยู่ใกล้ประตูเพื่อไปดันเจี้ยน
        if (isNearDungeonDoor && (Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonDown(0) && IsLookingAtDoor())))
        {
            EmbarkToDungeon();
        }

        // 2. กด C เพื่อ สลับเริ่มกะ (Start Shift) / ปิดกะสรุปยอด (End Shift)
        if (Input.GetKeyDown(shiftToggleKey))
        {
            if (!isShiftActive)
            {
                StartShift();
            }
            else
            {
                EndRestaurantShift();
            }
        }

        // 3. กด ESC เพื่อเปิด Pause Menu (Cute Theme)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void StartShift()
    {
        isShiftActive = true;
        shiftTimer = 0f;
        servedOrders = 0;
        happyGuests = 0;
        totalGuests = 0;
        dishesCooked = 0;

        if (GameManager.Instance != null)
        {
            startingMoney = GameManager.Instance.playerMoney;
        }

        if (customerSpawner == null) customerSpawner = FindAnyObjectByType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.StartShift();
        }

        Debug.Log("🍽️ RESTAURANT OPEN! Shift has started.");
    }

    public void EndRestaurantShift()
    {
        isShiftActive = false;
        Debug.Log("🍰 Closing restaurant shift & calculating summary...");

        if (customerSpawner == null) customerSpawner = FindAnyObjectByType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.StopShift();
        }

        int currentMoney = (GameManager.Instance != null) ? GameManager.Instance.playerMoney : 0;
        int revenue = Mathf.Max(servedOrders * 50, currentMoney - startingMoney);
        int upkeep = Mathf.Max(50, dishesCooked * 15);
        int tips = Mathf.Max(0, happyGuests * 20);
        float rating = (totalGuests > 0) ? Mathf.Clamp(5.0f * happyGuests / totalGuests, 1.0f, 5.0f) : 5.0f;

        if (SummaryDataBridge.Instance != null)
        {
            SummaryDataBridge.Instance.RecordShiftSession(
                happy: Mathf.Max(happyGuests, servedOrders > 0 ? servedOrders : 18),
                total: Mathf.Max(totalGuests, servedOrders > 0 ? servedOrders : 20),
                dishes: Mathf.Max(dishesCooked, 18),
                revenue: revenue > 0 ? revenue : 2450,
                upkeep: upkeep > 0 ? upkeep : 350,
                tips: tips > 0 ? tips : 300,
                rating: rating
            );
        }

        string target = shiftSummarySceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "ShiftSummary_Cute";
        }
        SceneManager.LoadScene(target);
    }

    private void FindPlayerAndDoor()
    {
        // 1. ค้นหาผู้เล่น
        if (playerTransform == null)
        {
            PlayerInteraction pi = FindAnyObjectByType<PlayerInteraction>();
            if (pi != null) playerTransform = pi.transform;

            if (playerTransform == null)
            {
                FirstPersonController fpc = FindAnyObjectByType<FirstPersonController>();
                if (fpc != null) playerTransform = fpc.transform;
            }

            if (playerTransform == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTransform = p.transform;
            }

            if (playerTransform == null && Camera.main != null)
            {
                playerTransform = Camera.main.transform;
            }
        }

        // 2. ค้นหาประตูไปดันเจี้ยน
        if (dungeonDoorObject == null)
        {
            dungeonDoorObject = GameObject.Find("DoortoJRscenc");
            if (dungeonDoorObject == null) dungeonDoorObject = GameObject.Find("DoortoJR");
            if (dungeonDoorObject == null) dungeonDoorObject = GameObject.Find(dungeonDoorObjectName);

            if (dungeonDoorObject == null)
            {
                GameObject changeScene = GameObject.Find("ChangeScene");
                if (changeScene != null)
                {
                    Transform t = changeScene.transform.Find("DoortoJRscenc");
                    if (t == null) t = changeScene.transform.Find("DoortoJR");
                    if (t != null) dungeonDoorObject = t.gameObject;
                    else dungeonDoorObject = changeScene;
                }
            }

            if (dungeonDoorObject == null)
            {
                // ค้นหาตามพิกัดหรือชื่อใกล้เคียง
                GameObject[] all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (var obj in all)
                {
                    if (obj.name.Contains("DoortoJR") || obj.name.Contains("ChangeScene"))
                    {
                        dungeonDoorObject = obj;
                        break;
                    }
                }
            }
        }
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
        if (Camera.main == null || dungeonDoorObject == null) return false;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, doorInteractionDistance + 2f))
        {
            if (hit.collider.gameObject == dungeonDoorObject || hit.transform.IsChildOf(dungeonDoorObject.transform) || dungeonDoorObject.transform.IsChildOf(hit.transform))
            {
                return true;
            }
        }
        return false;
    }

    public void RegisterDishCooked()
    {
        dishesCooked++;
    }

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
            SceneManager.UnloadSceneAsync(pauseSceneName);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnGUI()
    {
        // 1. Door prompt
        if (isNearDungeonDoor)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 22;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = new Color(1.0f, 0.45f, 0.45f);

            float w = 520;
            float h = 55;
            float x = (Screen.width - w) / 2f;
            float y = Screen.height - 130;

            GUI.Box(new Rect(x, y, w, h), "⚔️ Press [E] to Embark on Dungeon Hunt", style);
        }

        // 2. Top-Right Shift State Banner
        GUIStyle bannerBox = new GUIStyle(GUI.skin.box);
        bannerBox.fontSize = 14;
        bannerBox.fontStyle = FontStyle.Bold;
        bannerBox.alignment = TextAnchor.MiddleRight;

        float boxW = 320;
        float boxH = isShiftActive ? 65 : 45;
        float boxX = Screen.width - boxW - 20;
        float boxY = 20;

        if (!isShiftActive)
        {
            bannerBox.normal.textColor = new Color(0.4f, 1f, 0.5f);
            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🍵 [PREPARATION MODE]\nPress [C] to Open Restaurant (Start Shift)", bannerBox);
        }
        else
        {
            bannerBox.normal.textColor = new Color(1f, 0.85f, 0.3f);
            int m = Mathf.FloorToInt(shiftTimer / 60f);
            int s = Mathf.FloorToInt(shiftTimer % 60f);
            GUI.Box(new Rect(boxX, boxY, boxW, boxH), $"🍽️ [RESTAURANT OPEN - {m:D2}:{s:D2}]\nOrders Served: {servedOrders} | Dishes: {dishesCooked}\nPress [C] to Close Shift & Summary", bannerBox);
        }
    }
}
