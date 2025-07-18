using UnityEngine;
using System.Collections;

public class HealingPickup : MonoBehaviour
{
    [Range(0f, 1f)]
    public float healingPercentage = 0.25f;

    public string healingID; // <- Aggiunto
    private MessageDisplay messageDisplay;

    public string fullHealthMessage = "Il tuo stato di salute è al massimo, torna quando ricevi qualche danno!";
    private SaveController saveController;

    private IEnumerator Start()
    {
        messageDisplay = FindFirstObjectByType<MessageDisplay>();
        saveController = FindFirstObjectByType<SaveController>();

        // Aspetta che il SaveController abbia caricato i dati
        while (saveController != null && !saveController.isDataLoaded)
        {
            yield return null; // aspetta il prossimo frame
        }

        // Ora i dati sono caricati, quindi puoi controllare
        if (saveController != null && saveController.IsHealingUsed(healingID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            if (player.currentHealth >= player.maxHealth)
            {
                messageDisplay.ShowMessage(fullHealthMessage);
                return;
            }

            player.Heal(healingPercentage);
            SoundEffectManager.Play("HealingPointSound");

            // Salva come usato
            saveController.SetHealingUsed(healingID, true);

            Destroy(gameObject);
        }
    }
}