using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonFlowController : MonoBehaviour
{
    public static DungeonFlowController Instance { get; private set; }

    [Header("Door / Extraction Settings")]
    public string doorObjectName = "Door to SRN";
    public float interactionDistance = 2.5f;
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
    private Texture2D whiteTexture;

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

        if (doorObject == null)
        {
            doorObject = GameObject.Find("Door to SRN") ?? GameObject.Find("Door") ?? GameObject.Find(doorObjectName);

            if (doorObject == null)
            {
                GameObject[] all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (var obj in all)
                {
                    if (obj.name.Contains("Door") || obj.name.Contains("Exit") || obj.name.Contains("Portal"))
                    {
                        doorObject = obj;
                        break;
                    }
                }
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
        Debug.Log($"Enemy slain! Total session kills: {sessionKills}");
    }

    public void ExtractToRestaurant()
    {
        isExtracting = true;
        Debug.Log("Extracting from dungeon to restaurant...");

        int beefCount = Mathf.Max(InventoryManager.globalMeatCount, (InventoryManager.Instance != null ? InventoryManager.Instance.meatCount : 0));
        int porkCount = Mathf.Max(InventoryManager.globalPorkCount, (InventoryManager.Instance != null ? InventoryManager.Instance.porkCount : 0));

        Debug.Log($"Harvested in hunt -> Beef: {beefCount}, Pork: {porkCount}, Kills: {sessionKills}");

        SummaryDataBridge.RecordHuntSession(
            kills: Mathf.Max(sessionKills, beefCount + porkCount),
            beefAmount: beefCount,
            porkAmount: porkCount,
            timeSeconds: sessionDuration,
            damage: Mathf.Max(sessionDamage, (sessionKills + 1) * 1500)
        );

        InventoryManager.globalMeatCount = 0;
        InventoryManager.globalPorkCount = 0;

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

    private void DrawBox(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = old;
    }

    private void OnGUI()
    {
        if (Time.timeScale <= 0f) return;
        Scene pauseHunt = SceneManager.GetSceneByName(pauseSceneName);
        if (pauseHunt.isLoaded) return;

        // 1. Top-Right In-Theme Expedition Card
        float cardW = 380;
        float cardH = 44;
        float cardX = Screen.width - cardW - 16;
        float cardY = 16;

        DrawBox(new Rect(cardX, cardY, cardW, cardH), new Color(0.08f, 0.08f, 0.12f, 0.92f));

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 12;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.normal.textColor = new Color(1.0f, 0.45f, 0.45f);

        GUIStyle statsStyle = new GUIStyle(GUI.skin.label);
        statsStyle.fontSize = 11;
        statsStyle.fontStyle = FontStyle.Bold;
        statsStyle.alignment = TextAnchor.MiddleCenter;
        statsStyle.normal.textColor = Color.white;

        int m = Mathf.FloorToInt(sessionDuration / 60f);
        int s = Mathf.FloorToInt(sessionDuration % 60f);
        int beef = InventoryManager.Instance != null ? InventoryManager.Instance.meatCount : 0;
        int pork = InventoryManager.Instance != null ? InventoryManager.Instance.porkCount : 0;

        GUI.Label(new Rect(cardX, cardY + 4, cardW, 16), $"WILD EXPEDITION  •  Time: {m:D2}:{s:D2}", headerStyle);
        GUI.Label(new Rect(cardX, cardY + 22, cardW, 16), $"Kills: {sessionKills}   |   Beef: <color=#FFD700>{beef}</color>   |   Pork: <color=#FF9999>{pork}</color>", statsStyle);

        // 2. Door Extraction Prompt (When near door)
        if (isNearDoor && !isExtracting)
        {
            float promptW = 460;
            float promptH = 36;
            float promptX = (Screen.width - promptW) / 2f;
            float promptY = Screen.height - 110;

            DrawBox(new Rect(promptX, promptY, promptW, promptH), new Color(0.08f, 0.08f, 0.12f, 0.92f));

            GUIStyle promptStyle = new GUIStyle(GUI.skin.label);
            promptStyle.fontSize = 13;
            promptStyle.fontStyle = FontStyle.Bold;
            promptStyle.alignment = TextAnchor.MiddleCenter;
            promptStyle.normal.textColor = new Color(1f, 0.85f, 0.35f);

            GUI.Label(new Rect(promptX, promptY + 8, promptW, 20), "Press [E] to Return to Restaurant with Harvested Meats", promptStyle);
        }
    }
}
