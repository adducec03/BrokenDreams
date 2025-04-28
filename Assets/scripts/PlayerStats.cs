using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public int lives = 3; // Vite totali
    public Transform respawnPoint; // Ultimo checkpoint raggiunto
    public float maxHealth = 100f;
    public float currentHealth;

    public float maxShield = 50f;
    public float currentShield;

    public HealthBar healthBarGame;
    public ShieldBar shieldBarGame;
    public HealthBar healthBarMenu;
    public ShieldBar shieldBarMenu;
    public HeartsManager heartsManager;


    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        heartsManager = FindFirstObjectByType<HeartsManager>();
        heartsManager.UpdateHearts(lives);
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
        lives=lives-1;
        heartsManager.UpdateHearts(lives);

        if (lives > 0)
        {
            Debug.Log("Respawn al checkpoint. Vite rimaste: " + lives);
            Respawn();
        }
        else
        {
            Debug.Log("Game Over! Ricomincia il livello.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;
        currentHealth = maxHealth;
        currentShield = maxShield;

        healthBarGame.SetHealth(currentHealth, maxHealth);
        shieldBarGame.SetShield(currentShield, maxShield);
        healthBarMenu.SetHealth(currentHealth, maxHealth);
        shieldBarMenu.SetShield(currentShield, maxShield);
    }
}