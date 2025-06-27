using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Animazione di danno
        if (animator != null)
            animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notifico il salvataggio della morte del nemico
        EnemySaveState saveState = GetComponent<EnemySaveState>();
        if (saveState != null)
            saveState.MarkAsDefeated();

        // Blocca altre azioni e avvia animazione di morte
        if (animator != null)
            animator.SetTrigger("Die");

        // Disattiva AI o movimento
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // Opzionale: distruggi l’oggetto dopo un po’
        Destroy(gameObject, 1.5f);
    }
}