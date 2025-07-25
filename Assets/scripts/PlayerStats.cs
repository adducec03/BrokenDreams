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

    public float maxShield = 300f;
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
    public GameObject auraObject;
    public GameObject auraObjectUI;
    private AudioSource auraLoopSource;
    public bool hasShield = false;




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
        if (auraObject != null && auraObjectUI != null)
        {
            if (!hasShield)
            {
                auraObject.SetActive(false);
                auraObjectUI.SetActive(false);
            }
            else
            {
                auraObject.SetActive(true);
                auraObjectUI.SetActive(true);
            }
        }

    }

    public void Update()
    {
        //PER TESTING
        if (Input.GetKeyDown(KeyCode.X)) // o il tuo tasto di attacco
        {
            TakeDamage(1000f);
        }
        if (Input.GetKeyDown(KeyCode.C)) // o il tuo tasto di attacco
        {
            Heal(1f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        animator.SetTrigger("Hurt");

        // Usa lo scudo solo se attivo
        if (shieldBarGame.gameObject.activeSelf && currentShield > 0)
        {
            float shieldDamage = Mathf.Min(damage, currentShield);
            currentShield -= shieldDamage;
            damage -= shieldDamage;

            shieldBarGame.SetShield(currentShield, maxShield);
            shieldBarMenu.SetShield(currentShield, maxShield);
        }

        // Se rimane danno, applicalo alla salute
        if (damage > 0)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            healthBarGame.SetHealth(currentHealth, maxHealth);
            healthBarMenu.SetHealth(currentHealth, maxHealth);

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
        // Aggiorna tutte le barre
        maxHealth = 300f;
        currentHealth = maxHealth;
        currentShield = maxShield;
        shieldBarGame.SetShield(currentShield, maxShield);
        shieldBarMenu.SetShield(currentShield, maxShield);
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);

        // Mostra le barre dello scudo
        shieldBarGame.gameObject.SetActive(true);
        shieldBarMenu.gameObject.SetActive(true);

        // Abilita animazione Aura
        if (auraObject != null && auraObjectUI != null)
        {
            auraObject.SetActive(true);
            auraObjectUI.SetActive(true);
        }

        // Abilita suono Aura
        EnableAuraSound();

        // Aumenta il danno che può infliggere il player
        PlayerAttack attack = GetComponent<PlayerAttack>();
        attack.attackDamage = 30;
        hasShield = true;

    }

    public void Heal(float percent)
    {
        float amount = maxHealth * percent;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        healthBarGame.SetHealth(currentHealth, maxHealth);
        healthBarMenu.SetHealth(currentHealth, maxHealth);
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
            menuController.attackButtonUI.gameObject.SetActive(false);

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


    public void EnableAuraSound()
    {
        if (auraLoopSource == null)
        {
            auraLoopSource = SoundEffectManager.PlayLoopOnObject("AuraSound", this.gameObject);
        }
    }

    public void StopAuraSound()
    {
        if (auraLoopSource != null)
        {
            auraLoopSource.Stop();
            auraLoopSource = null;
        }
    }

    public bool HasShield()
    {
        return hasShield;
    }

    public void SetShieldState(bool state)
    {
        hasShield = state;
    }

}