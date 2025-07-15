using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAttack : MonoBehaviour
{
    public float visionRange = 6f;
    public float attackRange = 5f;
    public int attackDamage = 10;
    public float attackRate = 1f;
    public Transform player;

    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private Animator animator;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (player == null)
            Debug.LogError("Player non assegnato!");
    }

    void FixedUpdate()
    {
        if (player == null || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= visionRange)
        {
            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
            else
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                }
            }

            UpdateSpriteFlip();
        }
        else
        {
            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Ferma il movimento
        if (agent.isOnNavMesh)
            agent.isStopped = true;

        rb.linearVelocity = Vector2.zero;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(attackDamage);
        }

        SoundEffectManager.PlayAtPosition("SkeletonAttack", transform.position, 1f, 8f);

        Invoke(nameof(ResetAttack), 0.6f); // Adatta alla durata dell’animazione
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void UpdateSpriteFlip()
    {
        Vector3 direction = player.position - transform.position;
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}