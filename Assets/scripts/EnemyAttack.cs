using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAttack : MonoBehaviour
{
    public float visionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 4f;
    public int attackDamage = 10;
    public float attackRate = 1f;
    public float trailPersistence = 5f; // Quanto durano le tracce
    public float dashSpeed = 12f;
    public float dashCooldown = 2f;
    public float dashDistance = 3f;

    private float nextAttackTime = 0f;
    private float nextDashTime = 0f;
    public Transform player;
    private Transform target;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private LayerMask obstacleMask;
    private bool isDashing = false;
    private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (player == null)
        {
            Debug.LogError("Player non assegnato nell'Inspector!");
        }

        obstacleMask = LayerMask.GetMask("NotWalkable");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Durante il dash, ignora il NavMesh
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }

        // Cerca di inseguire direttamente il player se possibile
        if (IsPlayerVisible())
        {
            target = player;
        }
        else
        {
            // Se non vede il player, cerca l'ultima traccia visibile
            FindLatestVisibleTrail();
        }

        // Muovi il nemico verso il target (player o traccia)
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            // Dash di Attacco
            if (target == player && distance > attackRange && distance <= dashDistance && Time.time >= nextDashTime)
            {
                StartDash();
                return;
            }

            // Movimento normale
            if (distance > attackRange)
            {
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(target.position);
                }
            }
            else if (target == player && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashDirection = (player.position - transform.position).normalized;

        // Ferma temporaneamente l'agente durante il dash
        agent.enabled = false;

        // Ferma il dash dopo una breve durata
        Invoke(nameof(StopDash), dashDistance / dashSpeed);
        nextDashTime = Time.time + dashCooldown;
    }

    void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        // Riattiva l'agente NavMesh dopo il dash
        agent.enabled = true;
    }

    bool IsPlayerVisible()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= visionRange)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
            return hit.collider == null || hit.collider.CompareTag("Player");
        }

        return false;
    }

    void FindLatestVisibleTrail()
    {
        GameObject[] trails = GameObject.FindGameObjectsWithTag("TrailPoint");
        Transform latestTrail = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject trail in trails)
        {
            float distance = Vector2.Distance(transform.position, trail.transform.position);

            if (distance <= visionRange && HasLineOfSight(trail.transform.position))
            {
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    latestTrail = trail.transform;
                }
            }
        }

        target = latestTrail;
    }

    bool HasLineOfSight(Vector2 targetPos)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, targetPos, obstacleMask);
        return hit.collider == null || hit.collider.CompareTag("Player") || hit.collider.CompareTag("TrailPoint");
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
