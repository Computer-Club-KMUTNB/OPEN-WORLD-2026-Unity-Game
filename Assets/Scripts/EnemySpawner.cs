using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [Tooltip("ใส่ Prefab มอนสเตอร์ทั้ง 2 ชนิด")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Points")]
    [Tooltip("ลากจุดเกิดทั้งหมดมาใส่ (ระบบจะวนลูปใช้จุดเกิดอัตโนมัติ)")]
    public Transform[] spawnPoints;

    [Header("Wave Settings (3 Waves)")]
    [Tooltip("จำนวนมอนสเตอร์ใน Wave 1")]
    public int wave1EnemyCount = 1;
    [Tooltip("จำนวนมอนสเตอร์ใน Wave 2")]
    public int wave2EnemyCount = 3;
    [Tooltip("จำนวนมอนสเตอร์ใน Wave 3 (Final Wave)")]
    public int wave3EnemyCount = 5;

    [Header("Timing Settings")]
    [Tooltip("ระยะห่างเวลาการเกิดของแต่ละตัวในเวฟเดียวกัน (วินาที)")]
    public float spawnInterval = 0.5f;
    [Tooltip("ระยะเวลาพักก่อนเริ่มเวฟถัดไป (วินาที)")]
    public float timeBetweenWaves = 1.0f;

    [Header("UI & Door References")]
    public TMP_Text enemiesRemainingText;
    public DoorController exitDoor;

    private int totalEnemiesRemaining;
    private int currentWaveAlive = 0;
    private bool isWaveStarted = false;

    void Start()
    {
        // คำนวณยอดรวมศัตรูทั้งหมดทั้ง 3 เวฟอัตโนมัติ
        totalEnemiesRemaining = wave1EnemyCount + wave2EnemyCount + wave3EnemyCount;
        UpdateUI();
    }

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
        // --- Wave 1 ---
        yield return StartCoroutine(RunWave(wave1EnemyCount));
        yield return new WaitForSeconds(timeBetweenWaves);

        // --- Wave 2 ---
        yield return StartCoroutine(RunWave(wave2EnemyCount));
        yield return new WaitForSeconds(timeBetweenWaves);

        // --- Wave 3 ---
        yield return StartCoroutine(RunWave(wave3EnemyCount));

        // เคลียร์ครบทั้ง 3 เวฟ
        RoomCleared();
    }

    IEnumerator RunWave(int enemyCount)
    {
        if (enemyCount <= 0) yield break;

        currentWaveAlive = enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            // สลับวนลูปจุดเกิดตามจำนวน spawnPoints ที่มี (เช่น มี 3 จุด ตัวที่ 4 จะวนกลับมาเกิดจุดที่ 1)
            int spawnIndex = i % spawnPoints.Length;
            SpawnEnemyAtPoint(spawnIndex);

            // หากยังมีตัวถัดไปในเวฟ ให้เว้นระยะเวลาเล็กน้อย
            if (i < enemyCount - 1 && spawnInterval > 0f)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        // รอจนกว่ามอนสเตอร์ทั้งหมดในเวฟนี้จะถูกกำจัด
        yield return new WaitUntil(() => currentWaveAlive <= 0);
    }

    void SpawnEnemyAtPoint(int spawnIndex)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        // สุ่มเลือกระหว่าง Minotaur หรือ Monster2
        GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform targetPoint = spawnPoints[spawnIndex];

        GameObject enemyObj = Instantiate(selectedPrefab, targetPoint.position, targetPoint.rotation);

        EnemyAI ai = enemyObj.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.SetSpawner(this);
        }
    }

    public void OnEnemyKilled()
    {
        currentWaveAlive--;
        totalEnemiesRemaining--;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = $"Enemies Left: {totalEnemiesRemaining}";
        }
    }

    void RoomCleared()
    {
        Debug.Log("🎉 เคลียร์ทั้ง 3 Wave สำเร็จ!");

        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = "Room Cleared!";
        }

        if (exitDoor != null)
        {
            exitDoor.OpenDoor();
        }
    }
}