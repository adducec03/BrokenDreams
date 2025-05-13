using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAttack : MonoBehaviour
{
    public float visionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;
    public int attackDamage = 10;
    public float attackRate = 1f;

    private float nextAttackTime = 0f;
    public Transform player;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private LayerMask obstacleMask;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; // Imposta il movimento solo sul piano X-Y
        
        if (player == null)
        {
            Debug.LogError("Player non trovato!");
        }

        // Imposta il layer degli ostacoli (muri) come "NotWalkable"
        obstacleMask = LayerMask.GetMask("NotWalkable");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Controllo linea di vista usando Linecast
        bool hasLineOfSight = false;
        if (distance <= visionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            
            // Linecast tra nemico e giocatore per verificare se ci sono muri nel mezzo
            RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);

            // Se il raycast non colpisce nulla o colpisce il player, il nemico ha la linea di vista
            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                hasLineOfSight = true;
            }
        }

        if (hasLineOfSight)
        {
            if (distance > attackRange)
            {
                // Inseguimento continuo senza scatti
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(player.position);
                }
            }
            else
            {
                // Attacco
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }
        else
        {
            // Ferma il nemico se non vede il player
            agent.ResetPath();
        }
    }

    void Attack()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerStats stats = FindFirstObjectByType<PlayerStats>();
            if (stats != null)
            {
                Debug.Log("Danno inflitto al player.");
                stats.TakeDamage(attackDamage);
            }
        }
    }
}