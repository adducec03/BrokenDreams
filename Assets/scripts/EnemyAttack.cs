using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAttack : MonoBehaviour
{
    public float visionRange = 6f;
    public float attackRange = 0f;
    public int attackDamage = 10;
    public float attackRate = 1f;
    public Transform player;

    private Transform target;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private Animator animator;
    private LayerMask obstacleMask;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        obstacleMask = LayerMask.GetMask("NotWalkable");

        if (player == null)
            Debug.LogError("Player non assegnato!");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        UpdateSpriteFlip();

        // Target prioritario: il player visibile
        if (IsPlayerVisible())
        {
            target = player;
        }
        else
        {
            FindLatestVisibleTrail();
        }

        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance <= attackRange)
            {
                if (agent.hasPath)
                    agent.ResetPath();

                animator.SetFloat("Speed", 0);
                rb.linearVelocity = Vector2.zero;

                if (target == player && Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
            else
            {
                if (!isAttacking && agent.isOnNavMesh)
                {
                    agent.SetDestination(target.position);
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                }
            }
        }
        else
        {
            agent.ResetPath();
            animator.SetFloat("Speed", 0);
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(attackDamage);
        }

        Invoke(nameof(ResetAttack), 0.6f); // Adatta alla durata dell’animazione
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void UpdateSpriteFlip()
    {
        if (agent.velocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = agent.velocity.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    bool IsPlayerVisible()
    {
        if (Vector2.Distance(transform.position, player.position) > visionRange)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    void FindLatestVisibleTrail()
    {
        GameObject[] trails = GameObject.FindGameObjectsWithTag("TrailPoint");
        Transform latest = null;
        float shortestDist = Mathf.Infinity;

        foreach (var trail in trails)
        {
            float dist = Vector2.Distance(transform.position, trail.transform.position);
            if (dist <= visionRange && HasLineOfSight(trail.transform.position) && dist < shortestDist)
            {
                shortestDist = dist;
                latest = trail.transform;
            }
        }

        target = latest;
    }

    bool HasLineOfSight(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, pos, obstacleMask);
        return hit.collider == null || hit.collider.CompareTag("TrailPoint");
    }
}