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
        currentShield = 0f;

        // Disattiva UI scudo
        shieldBarGame.gameObject.SetActive(false);
        shieldBarMenu.gameObject.SetActive(false);

        heartsManager = FindFirstObjectByType<HeartsManager>();
        heartsManager.UpdateHearts(lives);
        
        // Inizializza barre della salute
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Danno ricevuto: " + damage);

        // Usa lo scudo solo se attivo
        if (shieldBarGame.gameObject.activeSelf && currentShield > 0)
        {
            float shieldDamage = Mathf.Min(damage, currentShield);
            currentShield -= shieldDamage;
            damage -= shieldDamage;

            shieldBarGame.SetShield(currentShield, maxShield);
            shieldBarMenu.SetShield(currentShield, maxShield);
            Debug.Log("Danno assorbito dallo scudo. Rimane: " + currentShield);
        }

        // Se rimane danno, applicalo alla salute
        if (damage > 0)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            healthBarGame.SetHealth(currentHealth, maxHealth);
            healthBarMenu.SetHealth(currentHealth, maxHealth);

            Debug.Log("Danno applicato alla salute. Rimane: " + currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        lives = lives - 1;
        heartsManager.UpdateHearts(lives);

        if (lives > 0)
        {
            Debug.Log("Respawn al checkpoint. Vite rimaste: " + lives);
            Respawn();
        }
        else
        {
            Debug.Log("Game Over! Ricomincia il livello.");

            SaveController saveController = FindFirstObjectByType<SaveController>();
            if (saveController != null)
            {
                saveController.DeleteSaveData();
            }
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

    public void EnableShield()
    {
        currentShield = maxShield;
        currentHealth = maxHealth;

        // Mostra le barre dello scudo
        shieldBarGame.gameObject.SetActive(true);
        shieldBarMenu.gameObject.SetActive(true);

        // Aggiorna tutte le barre
        shieldBarGame.SetShield(currentShield, maxShield);
        shieldBarMenu.SetShield(currentShield, maxShield);
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);

        Debug.Log("Scudo attivato. Valore: " + currentShield);
    }

}