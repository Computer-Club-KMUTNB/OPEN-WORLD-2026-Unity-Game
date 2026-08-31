using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonFlowController : MonoBehaviour
{
    public static DungeonFlowController Instance { get; private set; }

    [Header("Door / Extraction Settings")]
    [Tooltip("ชื่อ GameObject ของประตูวาร์ปกลับร้าน (ค่าเริ่มต้นคือ 'Door')")]
    public string doorObjectName = "Door";
    public float interactionDistance = 4.5f;
    public string summarySceneName = "ExpeditionSummary_Hunt";
    public string directRestaurantSceneName = "Dev_Restaurant_Flow";

    [Header("Pause Menu")]
    public string pauseSceneName = "PauseMenu_Hunt";

    [Header("Live Hunt Stats")]
    public int sessionKills = 0;
    public int sessionDamage = 0;
    public float sessionDuration = 0f;

    private Transform playerTransform;
    private GameObject doorObject;
    private bool isNearDoor = false;
    private float startTime;
    private bool isExtracting = false;

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
        startTime = Time.time;
        FindPlayerAndDoor();
    }

    private void Update()
    {
        sessionDuration = Time.time - startTime;

        if (playerTransform == null || doorObject == null)
        {
            FindPlayerAndDoor();
        }

        CheckDoorProximity();

        // กด E หรือคลิกซ้ายเมื่ออยู่ใกล้ประตู เพื่อวาร์ปกลับร้าน
        if (isNearDoor && (Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonDown(0) && IsLookingAtDoor())) && !isExtracting)
        {
            ExtractToRestaurant();
        }

        // กด ESC เพื่อเปิด Pause Menu (Hunt Theme)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void FindPlayerAndDoor()
    {
        // 1. ค้นหาผู้เล่น
        if (playerTransform == null)
        {
            PlayerHealth ph = FindAnyObjectByType<PlayerHealth>();
            if (ph != null) playerTransform = ph.transform;

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

        // 2. ค้นหาประตู
        if (doorObject == null)
        {
            doorObject = GameObject.Find("Door");
            if (doorObject == null) doorObject = GameObject.Find(doorObjectName);

            if (doorObject == null)
            {
                GameObject[] all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (var obj in all)
                {
                    if (obj.name.Equals("Door", System.StringComparison.OrdinalIgnoreCase) ||
                        obj.name.Contains("Exit") || obj.name.Contains("Portal"))
                    {
                        doorObject = obj;
                        break;
                    }
                }
            }

            // ถ้าไม่มีประตูในฉาก สร้างประตูฉุกเฉินใกล้ผู้เล่น
            if (doorObject == null && playerTransform != null)
            {
                doorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                doorObject.name = "Door";
                doorObject.transform.position = playerTransform.position + playerTransform.forward * 4f;
                doorObject.transform.localScale = new Vector3(2.5f, 3.5f, 0.5f);
                
                Collider c = doorObject.GetComponent<Collider>();
                if (c != null) c.isTrigger = true;

                Renderer r = doorObject.GetComponent<Renderer>();
                if (r != null) r.material.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
                
                Debug.Log($"Created fallback Dungeon Extraction Door at {doorObject.transform.position}");
            }
        }
    }

    private void CheckDoorProximity()
    {
        if (playerTransform == null || doorObject == null)
        {
            isNearDoor = false;
            return;
        }

        float dist = Vector3.Distance(playerTransform.position, doorObject.transform.position);
        isNearDoor = dist <= interactionDistance;
    }

    private bool IsLookingAtDoor()
    {
        if (Camera.main == null || doorObject == null) return false;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionDistance + 2f))
        {
            if (hit.collider.gameObject == doorObject || hit.transform.IsChildOf(doorObject.transform) || doorObject.transform.IsChildOf(hit.transform))
            {
                return true;
            }
        }
        return false;
    }

    public void RegisterEnemyKilled()
    {
        sessionKills++;
        sessionDamage += Random.Range(1200, 2200);
        Debug.Log($"⚔️ Enemy slain! Total session kills: {sessionKills}");
    }

    public void ExtractToRestaurant()
    {
        isExtracting = true;
        Debug.Log("🚪 Extracting from dungeon to restaurant...");

        // ดึงจำนวนเนื้อที่เก็บได้
        int beefCount = Mathf.Max(InventoryManager.globalMeatCount, (InventoryManager.Instance != null ? InventoryManager.Instance.meatCount : 0));
        int porkCount = Mathf.Max(InventoryManager.globalPorkCount, (InventoryManager.Instance != null ? InventoryManager.Instance.porkCount : 0));

        Debug.Log($"🏹 Harvested in hunt -> Beef: {beefCount}, Pork: {porkCount}, Kills: {sessionKills}");

        // บันทึกสถิติลง SummaryDataBridge
        SummaryDataBridge.RecordHuntSession(
            kills: Mathf.Max(sessionKills, beefCount + porkCount),
            beefAmount: beefCount,
            porkAmount: porkCount,
            timeSeconds: sessionDuration,
            damage: Mathf.Max(sessionDamage, (sessionKills + 1) * 1500)
        );

        // Reset inventory counter for next dungeon run
        InventoryManager.globalMeatCount = 0;
        InventoryManager.globalPorkCount = 0;

        // โหลดหน้าสรุปผลการล่า (Expedition Summary)
        string target = summarySceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = directRestaurantSceneName;
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
        if (Time.timeScale <= 0f) return;
        Scene pauseHunt = SceneManager.GetSceneByName(pauseSceneName);
        if (pauseHunt.isLoaded) return;

        if (isNearDoor && !isExtracting)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 22;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.yellow;

            float w = 540;
            float h = 55;
            float x = (Screen.width - w) / 2f;
            float y = Screen.height - 130;

            GUI.Box(new Rect(x, y, w, h), "🚪 Press [E] to Return to Restaurant with Loot", style);
        }

        // Top-Right Live Stats Banner
        GUIStyle statsBox = new GUIStyle(GUI.skin.box);
        statsBox.fontSize = 14;
        statsBox.fontStyle = FontStyle.Bold;
        statsBox.alignment = TextAnchor.MiddleRight;
        statsBox.normal.textColor = new Color(1f, 0.4f, 0.4f);

        int m = Mathf.FloorToInt(sessionDuration / 60f);
        int s = Mathf.FloorToInt(sessionDuration % 60f);
        int beef = InventoryManager.Instance != null ? InventoryManager.Instance.meatCount : 0;
        int pork = InventoryManager.Instance != null ? InventoryManager.Instance.porkCount : 0;

        GUI.Box(new Rect(Screen.width - 280, 20, 260, 50), $"🏹 [WILD EXPEDITION - {m:D2}:{s:D2}]\nKills: {sessionKills} | Beef: {beef} | Pork: {pork}", statsBox);
    }
}
