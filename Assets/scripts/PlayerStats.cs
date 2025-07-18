using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public int lives = 3; // Vite totali
    public Transform respawnPoint; // Ultimo checkpoint raggiunto
    public float maxHealth = 100f;
    public float currentHealth;

    public float maxShield = 200f;
    public float currentShield;

    public HealthBar healthBarGame;
    public ShieldBar shieldBarGame;
    public HealthBar healthBarMenu;
    public ShieldBar shieldBarMenu;
    public HeartsManager heartsManager;
    private Animator animator;
    private bool isDead = false;
    private Rigidbody2D rb;

    public GameObject gameOverPanel;
    public SoundtrackVolumeController normalMusicController;
    public SoundtrackVolumeController gameOverMusicController;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        animator = GetComponentInChildren<Animator>();

    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        animator.SetTrigger("Hurt");

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
        if (isDead) return;
        isDead = true;

        animator.ResetTrigger("Hurt");
        animator.SetTrigger("Die");
        lives--;
        heartsManager.UpdateHearts(lives);

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        SoundEffectManager.Play("PlayerDeath");

        StartCoroutine(DeathDelay());
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;
        currentHealth = maxHealth;
        currentShield = maxShield;
        isDead = false;

        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerAttack>().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        animator.Play("Idle");

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
        maxHealth = 300f;
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);

        Debug.Log("Scudo attivato. Valore: " + currentShield);
        Debug.Log("Salute aumentata. Valore: " + currentHealth);
    }

    public void Heal(float percent)
    {
        float amount = maxHealth * percent;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);
        Debug.Log($"Guarito di {amount}. Salute attuale: {currentHealth}/{maxHealth}");
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(1.2f); // durata dell’animazione "Die"

        if (lives > 0)
        {
            Respawn();
        }
        else
        {
            // Ferma la musica normale
            if (normalMusicController != null)
                normalMusicController.GetComponent<AudioSource>().Stop();

            // Avvia musica Game Over
            if (gameOverMusicController != null)
                gameOverMusicController.GetComponent<AudioSource>().Play();

            // Disabilita il PauseMenu
            menuController menuController = FindFirstObjectByType<menuController>();
            menuController.menuButton.gameObject.SetActive(false);

            // Mostra la UI del GameOver
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Ferma il tempo
            Time.timeScale = 0f;

            // Elimina il salvataggio

            SaveController saveController = FindFirstObjectByType<SaveController>();
            if (saveController != null)
            {
                saveController.DeleteSaveData();
            }
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Riattiva il tempo

        // Ferma musica Game Over, riavvia quella normale
        if (gameOverMusicController != null)
            gameOverMusicController.GetComponent<AudioSource>().Stop();

        if (normalMusicController != null)
            normalMusicController.GetComponent<AudioSource>().Play();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Riattiva il tempo nel caso fosse fermo

        if (gameOverMusicController != null)
            gameOverMusicController.GetComponent<AudioSource>().Stop();

        SceneManager.LoadScene("MainMenu");
    }

}