using UnityEngine;
using UnityEngine.AI;
using System.Collections;


[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    public Transform player;
    public float visionRange = 8f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public int maxHealth = 100;
    public int attackDamage = 20;
    private int currentHealth;
    private Animator animator;
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool isDead = false;
    public GameObject minionPrefab;
    public Transform[] spawnPoints;
    public float summonInterval = 10f;
    public HealthBar healthBarBoss;
    public GameObject healthBarUI;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Debug.LogError("Boss non trovato sulla NavMesh!");
        }

    }

    public void StartSummoning()
    {
        if (!isDead)
            StartCoroutine(SpawnMinionsRoutine());
    }


    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= visionRange)
        {
            if (distance <= attackRange && Time.time >= lastAttackTime && !isAttacking)
            {
                Attack();
            }
            else if (!isAttacking)
            {
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(player.position);
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                }
            }
        }
        else
        {
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);
        }

        // Flip sprite in base alla direzione
        Vector3 direction = player.position - transform.position;
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    IEnumerator SpawnMinionsRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(summonInterval);
            SpawnMinion();
        }
    }


    void SpawnMinion()
    {
        int i = Random.Range(0, spawnPoints.Length);
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("spawnPoints è nullo o vuoto!");
            return;
        }

        for (int j = 0; j < spawnPoints.Length; j++)
        {
            if (spawnPoints[j] == null)
            {
                Debug.LogError($"spawnPoints[{j}] non è assegnato!");
                return;
            }
        }
        Instantiate(minionPrefab, spawnPoints[i].position, Quaternion.identity);
    }

    void Attack()
    {
        isAttacking = true;
        agent.ResetPath();
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        // Infliggi danno dopo breve delay (match animazione)
        Invoke(nameof(DealDamage), 0.5f);
        Invoke(nameof(EndAttack), 1f);


    }

    void DealDamage()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(attackDamage);
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        animator.SetTrigger("Hurt");
        healthBarBoss.SetHealth(currentHealth, maxHealth);

        Debug.Log("Boss ha subito danni: " + amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        isDead = true;
        SoundEffectManager.Play("GolemDeath");
        agent.ResetPath();
        animator.SetFloat("Speed", 0f);
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hurt");
        animator.SetTrigger("Die");
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f);
        StartCoroutine(RemoveBossHealthBarWithDelay());
    }
    
    private IEnumerator RemoveBossHealthBarWithDelay()
    {
        yield return new WaitForSeconds(1f);
        healthBarUI.SetActive(false);
    }
}