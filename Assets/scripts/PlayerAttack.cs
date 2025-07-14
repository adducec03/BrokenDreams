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

    public void TriggerAttack()
    {
        Attack();
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
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        bool hasHit = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            // Tenta di colpire un nemico normale
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                hasHit = true;
                continue;
            }

            // Tenta di colpire il boss
            BossAI boss = enemy.GetComponent<BossAI>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
                hasHit = true;
            }

            // Tenta di colpire uno slime
            SlimesAI minion = enemy.GetComponent<SlimesAI>();
            if (minion != null)
            {
                minion.TakeDamage(attackDamage);
                hasHit = true;
            }
        }

        // 🎵 Suono in base all'esito
        if (hasHit)
        {
            SoundEffectManager.Play("PlayerSwordHit"); // suono quando colpisci
        }
        else
        {
            SoundEffectManager.Play("PlayerSwordSwoosh"); // suono quando manchi
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