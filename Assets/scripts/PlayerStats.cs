using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public float maxShield = 50f;
    public float currentShield;

    public HealthBar healthBarGame;
    public ShieldBar shieldBarGame;
    public HealthBar healthBarMenu;
    public ShieldBar shieldBarMenu;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    public void TakeDamage(float damage)
{
    // 1. Riduci prima lo scudo
    if (currentShield > 0)
    {
        float shieldDamage = Mathf.Min(damage, currentShield);
        currentShield -= shieldDamage;
        damage -= shieldDamage; // Rimuovi la parte assorbita dallo scudo
        shieldBarGame.SetShield(currentShield, maxShield);
        shieldBarMenu.SetShield(currentShield, maxShield);
    }

    // 2. Se resta del danno, colpisci la salute
    if (damage > 0)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);
    }

    // 3. Controllo morte
    if (currentHealth <= 0)
    {
        Die();
    }
}


    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spazio premuto, danno applicato!");
            TakeDamage(15);
        }
    }

    void Die()
    {
        Debug.Log("Player is dead");
        // Aggiungi animazioni o logica di morte qui
    }
}