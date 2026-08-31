using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Combat Settings")]
    public float attackDamage = 15f;
    public float attackRange = 2.2f;
    public float attackRate = 1.6f;
    [Tooltip("ระยะเวลาหน่วงให้แอนิเมชันฟันลงมาก่อน ค่อยหักเลือดผู้เล่น (วินาที)")]
    public float damageDelay = 0.5f; 
    private float nextAttackTime = 0f;

    [Header("Animation Attack Cycling")]
    public int totalAttackAnimations = 2;
    private int currentAttackIndex = 0;

    [Header("Drops")]
    public GameObject meatDropPrefab;

    [Header("Death Settings")]
    public float deathDelay = 2.0f;

    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider col;
    private EnemySpawner spawnerRef;
    private bool isDead = false;

    public void SetSpawner(EnemySpawner spawner)
    {
        spawnerRef = spawner;
    }

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<Collider>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (isDead || player == null || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // หยุดเดิน หันหน้าเข้าหาผู้เล่น
            agent.isStopped = true;
            LookAtPlayer();

            if (anim != null) anim.SetBool("isMoving", false);

            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(PerformAttackRoutine());
                nextAttackTime = Time.time + attackRate;
            }
        }
        else
        {
            // วิ่งไล่ตามผู้เล่น
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (anim != null) anim.SetBool("isMoving", true);
        }
    }

    void LookAtPlayer()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);
    }

    IEnumerator PerformAttackRoutine()
    {
        // 1. สั่งเล่นท่าแอนิเมชันฟัน
        if (anim != null)
        {
            anim.SetInteger("attackIndex", currentAttackIndex);
            anim.SetTrigger("Attack");

            currentAttackIndex = (currentAttackIndex + 1) % totalAttackAnimations;
        }

        // 2. รอจังหวะให้ขวาน/เท้าฟันสับลงมาก่อน (ตามเวลา damageDelay)
        yield return new WaitForSeconds(damageDelay);

        // 3. ตรวจสอบว่าถ้ายังไม่ตาย และผู้เล่นยังอยู่ในระยะ จึงหักเลือดจริง
        if (!isDead && player != null && Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void Damage(float amount) => TakeDamage(amount);
    public void Damage(int amount) => TakeDamage(amount);
    public void makeDamage(float amount) => TakeDamage(amount);
    public void makeDamage(int amount) => TakeDamage(amount);

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StartCoroutine(DieSequence());
        }
    }

    IEnumerator DieSequence()
    {
        isDead = true;

        if (spawnerRef != null)
        {
            spawnerRef.OnEnemyKilled();
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        if (col != null)
        {
            col.enabled = false;
        }

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        if (meatDropPrefab == null)
        {
            meatDropPrefab = GameObject.Find("MeatDrop") ?? GameObject.Find("MeatPickup") ?? Resources.Load<GameObject>("MeatDrop");
        }

        if (meatDropPrefab != null)
        {
            Instantiate(meatDropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Debug.Log($"🥩 Spawn Meat Drop at {transform.position}");
        }
        else
        {
            Debug.LogWarning("⚠️ meatDropPrefab ยังว่างใน EnemyAI กรุณากลับไปผูก Prefab Meat ลงใน Inspector");
        }

        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }
}