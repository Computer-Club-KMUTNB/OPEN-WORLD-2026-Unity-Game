using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [Tooltip("Drag monster prefabs here (Minotaur, Monster2, etc.)")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Points  ← Drag spawn transforms here (add / delete freely)")]
    [Tooltip("Add as many spawn points as you want. Enemies cycle through them in order.")]
    public Transform[] spawnPoints;

    [Header("Wave Settings (3 Waves)")]
    public int wave1EnemyCount = 2;
    public int wave2EnemyCount = 5;
    public int wave3EnemyCount = 7;

    [Header("Timing Settings")]
    public float spawnInterval    = 0.5f;
    public float timeBetweenWaves = 2.0f;

    [Header("Room Trigger")]
    [Tooltip("Set the Collider on THIS GameObject to Trigger=true so the wave starts when the player walks in. " +
             "Leave unchecked to start on scene load instead.")]
    public bool startOnPlayerEnter = true;

    [Header("UI Reference")]
    public TMP_Text enemiesRemainingText;

    [Header("Door to Unlock  ← Drag the Door GameObject here")]
    [Tooltip("Drag the Door1 / Door2 / Door1(2) object from the scene hierarchy here")]
    public GameObject exitDoorObject;           // ← drag any door GameObject
    private DoorController exitDoor;            // resolved automatically

    [Header("Stage-Clear Effects")]
    [Tooltip("Optional particle system to play on room clear (drag VFX prefab/object here)")]
    public ParticleSystem clearParticles;
    public float clearShakeDuration  = 1.5f;
    public float clearShakePosForce  = 0.06f;
    public float clearShakeRotForce  = 1.2f;

    // ── internals ──────────────────────────────────────────────
    private int   totalEnemiesRemaining;
    private int   currentWaveAlive = 0;
    private bool  isWaveStarted    = false;

    // ── Screen-flash overlay ────────────────────────────────────
    private Texture2D flashTex;
    private float     flashAlpha   = 0f;
    private bool      doFlash      = false;

    // ───────────────────────────────────────────────────────────
    void Awake()
    {
        flashTex = new Texture2D(1, 1);
        flashTex.SetPixel(0, 0, Color.white);
        flashTex.Apply();
    }

    void Start()
    {
        // Resolve DoorController from the dragged GameObject
        if (exitDoorObject != null)
        {
            exitDoor = exitDoorObject.GetComponent<DoorController>();
            if (exitDoor == null)
                exitDoor = exitDoorObject.GetComponentInChildren<DoorController>();
            if (exitDoor == null)
                exitDoor = exitDoorObject.GetComponentInParent<DoorController>();

            // Component was stripped by scene repair — add it back automatically
            if (exitDoor == null)
            {
                exitDoor = exitDoorObject.AddComponent<DoorController>();
                Debug.Log($"EnemySpawner: DoorController was missing on '{exitDoorObject.name}' — added automatically.");
            }
        }

        totalEnemiesRemaining = wave1EnemyCount + wave2EnemyCount + wave3EnemyCount;
        UpdateUI();

        // If not trigger-based, start immediately
        if (!startOnPlayerEnter)
        {
            StartWave();
        }
    }

    // ── Trigger: player walks into room ────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!startOnPlayerEnter) return;
        if (!other.CompareTag("Player")) return;

        // Guard against overlapping triggers: only fire if THIS spawner
        // is the closest one to the player right now.
        if (!IsNearestSpawnerToPlayer(other.transform.position)) return;

        StartWave();
    }

    bool IsNearestSpawnerToPlayer(Vector3 playerPos)
    {
        EnemySpawner[] all = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        float myDist = Vector3.Distance(transform.position, playerPos);

        foreach (var s in all)
        {
            if (s == this) continue;
            if (s.isWaveStarted) continue;          // already running — skip
            if (Vector3.Distance(s.transform.position, playerPos) < myDist)
                return false;                        // another unstarted spawner is closer
        }
        return true;
    }

    // ── Wave Control ───────────────────────────────────────────
    public void StartWave()
    {
        if (isWaveStarted) return;
        isWaveStarted = true;

        totalEnemiesRemaining = wave1EnemyCount + wave2EnemyCount + wave3EnemyCount;
        UpdateUI();
        StartCoroutine(ExecuteWaveSequence());
    }

    IEnumerator ExecuteWaveSequence()
    {
        yield return StartCoroutine(RunWave(wave1EnemyCount));
        yield return new WaitForSeconds(timeBetweenWaves);

        yield return StartCoroutine(RunWave(wave2EnemyCount));
        yield return new WaitForSeconds(timeBetweenWaves);

        yield return StartCoroutine(RunWave(wave3EnemyCount));

        RoomCleared();
    }

    IEnumerator RunWave(int enemyCount)
    {
        if (enemyCount <= 0) yield break;

        currentWaveAlive = enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int spawnIndex = i % spawnPoints.Length;
                SpawnEnemyAtPoint(spawnIndex);
            }

            if (i < enemyCount - 1 && spawnInterval > 0f)
                yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitUntil(() => currentWaveAlive <= 0);
    }

    void SpawnEnemyAtPoint(int spawnIndex)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (spawnPoints  == null || spawnPoints.Length  == 0) return;

        GameObject prefab      = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform  spawnPoint  = spawnPoints[spawnIndex];

        GameObject enemyObj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        EnemyAI ai = enemyObj.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.SetSpawner(this);
        }
    }

    // ── Enemy callback ─────────────────────────────────────────
    public void OnEnemyKilled()
    {
        currentWaveAlive--;
        totalEnemiesRemaining--;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (enemiesRemainingText != null && enemiesRemainingText.gameObject.activeSelf)
            enemiesRemainingText.gameObject.SetActive(false);
    }

    // ── Room Cleared ───────────────────────────────────────────
    void RoomCleared()
    {
        Debug.Log("Room Cleared! All waves defeated.");

        if (enemiesRemainingText != null && enemiesRemainingText.gameObject.activeSelf)
            enemiesRemainingText.gameObject.SetActive(false);

        // Fallback: find nearest door if inspector field is empty
        if (exitDoor == null)
            exitDoor = FindNearestDoor();

        if (exitDoor != null)
        {
            exitDoor.OpenDoor();
            Debug.Log($"🚪 Unlocked door: {exitDoor.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("EnemySpawner: exitDoor not assigned and no DoorController found nearby!");
        }

        // ── Stage-clear effects ──
        StartCoroutine(StageClearEffects());
    }

    IEnumerator StageClearEffects()
    {
        // 1. Camera shake (like door open shake but stronger)
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(clearShakeDuration, clearShakePosForce, clearShakeRotForce);

        // 2. Particle burst
        if (clearParticles != null)
        {
            clearParticles.gameObject.SetActive(true);
            clearParticles.Play();
        }

        // 3. Golden screen flash
        doFlash  = true;
        flashAlpha = 0.55f;
        float fadeTime = 1.2f;
        while (flashAlpha > 0f)
        {
            flashAlpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        flashAlpha = 0f;
        doFlash    = false;
    }

    void OnGUI()
    {
        if (!doFlash || flashAlpha <= 0f) return;
        Color c = new Color(1f, 0.92f, 0.2f, flashAlpha); // golden flash
        GUI.color = c;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), flashTex);
        GUI.color = Color.white;
    }

    // ── Door fallback finder ───────────────────────────────────
    DoorController FindNearestDoor()
    {
        DoorController[] all     = FindObjectsByType<DoorController>(FindObjectsSortMode.None);
        DoorController   nearest = null;
        float            minDist = float.MaxValue;

        foreach (var d in all)
        {
            float dist = Vector3.Distance(transform.position, d.transform.position);
            if (dist < minDist) { minDist = dist; nearest = d; }
        }
        return nearest;
    }
}