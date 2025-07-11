using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;
    public Transform attackPoint;

    private Vector2 lastMoveDirection = Vector2.right; // default verso destra
    private PlayerMovement movementScript;
    private Animator animator;


    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        UpdateAttackPointDirection();

        if (Input.GetKeyDown(KeyCode.Z)) // o il tuo tasto di attacco
        {
            Attack();
        }
    }

    void UpdateAttackPointDirection()
    {
        Vector2 inputDir = movementScript.GetLastDirection(); // metodo che restituisce l’ultima direzione

        if (inputDir != Vector2.zero)
        {
            lastMoveDirection = inputDir.normalized;
        }

        // Sposta il punto d’attacco davanti al player
        attackPoint.localPosition = lastMoveDirection * 0.5f;
    }

    void Attack()
{
    animator.SetTrigger("Attack");

    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

    foreach (Collider2D enemy in hitEnemies)
    {
        // Tenta di colpire un nemico normale
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
            continue;
        }

        // Tenta di colpire il boss
        BossAI boss = enemy.GetComponent<BossAI>();
        if (boss != null)
        {
            boss.TakeDamage(attackDamage);
        }
    }
}

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}