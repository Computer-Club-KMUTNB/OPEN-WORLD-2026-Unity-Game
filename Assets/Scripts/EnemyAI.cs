using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Combat Settings")]
    public float attackDamage = 15f;
    public float attackRange = 1.8f;
    public float attackRate = 1.2f;
    private float nextAttackTime = 0f;

    [Header("Drops")]
    public GameObject meatDropPrefab;

    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private EnemySpawner spawnerRef; // อ้างอิงถึงตัวเสก

    public void SetSpawner(EnemySpawner spawner)
    {
        spawnerRef = spawner;
    }

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            AttackPlayer();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            nextAttackTime = Time.time + attackRate;
        }
    }

    public void Damage(float amount) => TakeDamage(amount);
    public void Damage(int amount) => TakeDamage(amount);
    public void makeDamage(float amount) => TakeDamage(amount);
    public void makeDamage(int amount) => TakeDamage(amount);

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // แจ้ง Spawner ว่าศัตรูตัวนี้ตายแล้ว
        if (spawnerRef != null)
        {
            spawnerRef.OnEnemyKilled();
        }

        if (meatDropPrefab != null)
        {
            Instantiate(meatDropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}