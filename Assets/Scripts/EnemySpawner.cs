using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int totalEnemiesThisWave = 5;
    public float spawnInterval = 2.0f;
    public int maxAliveEnemies = 3;

    [Header("UI & Door References")]
    [Tooltip("ลาก TextMeshPro แสดงจำนวนศัตรูมาใส่")]
    public TextMeshProUGUI enemiesRemainingText;
    [Tooltip("ลาก ประตูทางออก ที่ต้องการให้เปิดเมื่อเคลียร์จบมาใส่")]
    public DoorController exitDoor;

    private int spawnedCount = 0;
    private int currentAliveCount = 0;
    private int enemiesKilled = 0;
    private bool isSpawning = false;

    // ลบการ StartWave ออกจาก Start() เพื่อรอให้ Trigger เป็นตัวสั่งเริ่ม
    void Start()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.gameObject.SetActive(false); // ซ่อน UI ไว้ก่อนจนกว่าจะเริ่มเวฟ
        }
    }

    public void StartWave()
    {
        spawnedCount = 0;
        currentAliveCount = 0;
        enemiesKilled = 0;
        isSpawning = true;

        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.gameObject.SetActive(true);
            UpdateRemainingUI();
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (spawnedCount < totalEnemiesThisWave)
        {
            if (currentAliveCount < maxAliveEnemies && spawnPoints.Length > 0)
            {
                SpawnSingleEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
        isSpawning = false;
    }

    void SpawnSingleEnemy()
    {
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newEnemy = Instantiate(enemyPrefab, randomPoint.position, randomPoint.rotation);

        EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.SetSpawner(this);
        }

        spawnedCount++;
        currentAliveCount++;
    }

    public void OnEnemyKilled()
    {
        currentAliveCount--;
        enemiesKilled++;
        UpdateRemainingUI();

        if (!isSpawning && currentAliveCount <= 0)
        {
            OnWaveCleared();
        }
    }

    void UpdateRemainingUI()
    {
        if (enemiesRemainingText != null)
        {
            int remaining = totalEnemiesThisWave - enemiesKilled;
            enemiesRemainingText.text = $"Enemies Left: {remaining}";
        }
    }

    void OnWaveCleared()
    {
        Debug.Log("🎉 เคลียร์ห้องสำเร็จ!");
        
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = "ROOM CLEARED!";
        }

        // สั่งเปิดประตูทางออก
        if (exitDoor != null)
        {
            exitDoor.OpenDoor();
        }
    }
}