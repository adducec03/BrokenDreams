using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAttack : MonoBehaviour
{
    public float visionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;
    public int attackDamage = 10;
    public float attackRate = 1f;

    private float nextAttackTime = 0f;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            Debug.LogError("Player non trovato!");
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= visionRange)
        {
            // Se il player è nel raggio visivo
            if (distance > attackRange)
            {
                // Inseguimento
                Vector2 direction = (player.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
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