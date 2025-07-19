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
    public int maxHealth = 1000;
    public int attackDamage = 30;
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
    public GameObject crackEffectPrefab;
    public GameObject spawnEffectPrefab;
    public SceneTransitionManagerLevel1 SceneTransitionManagerLevel1;
    private AudioSource ambientLoopSource;
    private bool hasIncreasedSpawnRate = false;
    public GameObject auraObject;
    private AudioSource breathLoopSource;




    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        EnableBreathSound();
        if (auraObject != null)
        {
            auraObject.SetActive(false);
        }

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
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("spawnPoints è nullo o vuoto!");
            return;
        }

        // Controlla che tutti i punti siano validi
        for (int j = 0; j < spawnPoints.Length; j++)
        {
            if (spawnPoints[j] == null)
            {
                Debug.LogError($"spawnPoints[{j}] non è assegnato!");
                return;
            }
        }

        int i = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[i].position;

        StartCoroutine(SpawnMinionWithEffect(spawnPosition));
    }

    IEnumerator SpawnMinionWithEffect(Vector3 position)
    {
        // Istanzia l'effetto visivo di spawn (particelle, flash, ecc.)
        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, position, Quaternion.identity);
            SoundEffectManager.Play("SpawnEffect");
        }

        // Attendi leggermente (es. 0.3 secondi) prima di generare lo slime
        yield return new WaitForSeconds(0.3f);

        Instantiate(minionPrefab, position, Quaternion.identity);
    }

    void Attack()
    {
        isAttacking = true;
        agent.ResetPath();
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Attack");
        Invoke(nameof(SpawnCrackEffect), 0.5f);
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

        // Controlla se la salute è scesa sotto metà e se non abbiamo già cambiato la frequenza
        if (!hasIncreasedSpawnRate && currentHealth <= maxHealth / 2)
        {
            hasIncreasedSpawnRate = true;
            summonInterval /= 2f;  // dimezza l'intervallo → raddoppia la frequenza
            if (auraObject != null)
            {
                auraObject.SetActive(true);
            }

            Debug.Log("⚠️ Boss sotto metà vita! Spawn più frequente: " + summonInterval + "s");
        }

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
        if (auraObject != null)
        {
            auraObject.SetActive(false);
        }
        if (breathLoopSource != null)
        {
            breathLoopSource.Stop();
            Destroy(breathLoopSource.gameObject);
        }
        Destroy(gameObject, 10f);
        StartCoroutine(RemoveBossHealthBarWithDelay());
        StartCoroutine(HandleAfterBossDeath());
    }

    IEnumerator HandleAfterBossDeath()
    {
        // Blocca il player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement controller = player.GetComponent<PlayerMovement>();
            if (controller != null)
            {
                controller.enabled = false; // Disattiva input
            }

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Animator anim = player.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetBool("isWalking", false); // imposta animazione su idle
            }

        }

        yield return new WaitForSeconds(2f); // aspetta un attimo per atmosfera

        KillAllSlimes();

        StartCoroutine(LoadMainMenuWithDelay());
    }

    void KillAllSlimes()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            SlimesAI slime = enemy.GetComponent<SlimesAI>();
            if (slime != null)
            {
                slime.Die();
            }
        }
    }

    private IEnumerator RemoveBossHealthBarWithDelay()
    {
        yield return new WaitForSeconds(1f);
        healthBarUI.SetActive(false);
    }

    void SpawnCrackEffect()
    {
        if (crackEffectPrefab != null)
        {
            // Calcola direzione frontale basata sulla scala X
            float direction = Mathf.Sign(transform.localScale.x); // +1 = destra, -1 = sinistra

            // Calcola posizione più avanti nella direzione in cui guarda
            Vector3 spawnOffset = new Vector3(direction * 1f, -0.5f, 0); // avanti e leggermente in basso
            Vector3 spawnPosition = transform.position + spawnOffset;

            Instantiate(crackEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }


    IEnumerator LoadMainMenuWithDelay()
    {
        yield return new WaitForSeconds(2.5f); // tempo per l'animazione

        if (SceneTransitionManagerLevel1 != null)
        {
            SceneTransitionManagerLevel1.StartSceneTransition("MainMenu");
        }
        else
        {
            Debug.LogError("SceneTransitionManager non trovato!");
        }
    }

    public void EnableBreathSound()
    {
        if (breathLoopSource == null)
        {
            breathLoopSource = SoundEffectManager.PlayLoopAtPosition("GolemBreath", transform.position, 12f, 50f);
        }
    }

    public void StopBreathSound()
    {
        if (breathLoopSource != null)
        {
            breathLoopSource.Stop();
            breathLoopSource = null;
        }
    }
}