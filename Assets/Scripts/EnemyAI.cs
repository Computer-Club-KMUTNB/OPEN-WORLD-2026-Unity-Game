using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform player;
    public float attackRange = 2.8f;
    public float moveSpeed = 3.5f;

    [Header("Combat Settings")]
    public float attackDamage = 25f;
    public float attackCooldown = 1.6f;
    public float damageDelay = 0.45f;
    private float lastAttackTime;
    private bool hasDealtDamageThisAttack = false;

    [Header("Stats")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float deathDelay = 1.5f;

    [Header("Loot Drop")]
    public GameObject meatDropPrefab;

    [Header("References")]
    private NavMeshAgent agent;
    private Animator anim;
    private Collider col;
    private bool isDead = false;
    private EnemySpawner spawnerRef;

    public void SetSpawner(EnemySpawner spawner)
    {
        spawnerRef = spawner;
    }

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = Mathf.Max(1.0f, attackRange - 0.5f);
        }

        FindPlayer();
    }

    private void FindPlayer()
    {
        if (player != null) return;

        PlayerHealth ph = FindAnyObjectByType<PlayerHealth>();
        if (ph != null) player = ph.transform;
        else
        {
            FirstPersonController fpc = FindAnyObjectByType<FirstPersonController>();
            if (fpc != null) player = fpc.transform;
            else if (Camera.main != null) player = Camera.main.transform;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            if (anim != null)
            {
                anim.SetBool("isMoving", true);
            }
        }
        else
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            if (anim != null)
            {
                anim.SetBool("isMoving", false);
            }

            Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookPos);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        hasDealtDamageThisAttack = false;

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        // หน่วงเวลารอจังหวะง้างฟันของแอนิเมชัน
        yield return new WaitForSeconds(damageDelay);

        if (!hasDealtDamageThisAttack && !isDead)
        {
            DealDamageToPlayer();
        }
    }

    // เรียกได้ทั้งจาก Coroutine และ Animation Event
    public void DealDamage()
    {
        if (!hasDealtDamageThisAttack && !isDead)
        {
            DealDamageToPlayer();
        }
    }

    private void DealDamageToPlayer()
    {
        if (player == null) FindPlayer();
        if (player == null || isDead) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange + 1.2f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>() ?? player.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                hasDealtDamageThisAttack = true;
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Enemy dealt {attackDamage} DMG to player! (Player HP: {playerHealth.currentHealth})");
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

        if (DungeonFlowController.Instance != null)
        {
            DungeonFlowController.Instance.RegisterEnemyKilled();
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

        bool isMonster2 = gameObject.name.ToLower().Contains("monster2") || gameObject.name.ToLower().Contains("pork");

        if (meatDropPrefab == null)
        {
            string prefabToLook = isMonster2 ? "Pork" : "Meat";
            meatDropPrefab = Resources.Load<GameObject>(prefabToLook) ?? GameObject.Find(prefabToLook);
        }

        if (meatDropPrefab != null)
        {
            GameObject drop = Instantiate(meatDropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            ItemPickup pickup = drop.GetComponentInChildren<ItemPickup>();
            if (pickup == null)
            {
                pickup = drop.AddComponent<ItemPickup>();
                pickup.itemType = isMonster2 ? ItemPickup.ItemType.Pork : ItemPickup.ItemType.Meat;
                pickup.amount = 1;
            }
            Debug.Log($"Spawned Loot Drop ({pickup.itemType}) at {transform.position}");
        }
        else
        {
            GameObject meatObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            meatObj.name = isMonster2 ? "RawPorkDrop" : "RawBeefDrop";
            meatObj.transform.position = transform.position + Vector3.up * 0.5f;
            meatObj.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
            
            ItemPickup mp = meatObj.AddComponent<ItemPickup>();
            mp.itemType = isMonster2 ? ItemPickup.ItemType.Pork : ItemPickup.ItemType.Meat;
            mp.amount = 1;

            Renderer mr = meatObj.GetComponent<Renderer>();
            if (mr != null) mr.material.color = isMonster2 ? new Color(0.95f, 0.6f, 0.6f) : new Color(0.85f, 0.2f, 0.2f);
            
            Debug.Log($"Created Loot Drop ({mp.itemType}) at {transform.position}");
        }

        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }
}