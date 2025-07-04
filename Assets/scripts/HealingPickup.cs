using UnityEngine;

public class HealingPickup : MonoBehaviour
{
    [Range(0f, 1f)]
    public float healingPercentage = 0.25f;

    private MessageDisplay messageDisplay; // L'oggetto per far stampare dei log direttamente sullo schermo

    public string fullHealthMessage = "Il tuo stato di salute è al massimo, torna quando ricevi qualche danno!";


    void Start()
    {
        messageDisplay = FindFirstObjectByType<MessageDisplay>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            if (player.currentHealth >= player.maxHealth)
            {
                messageDisplay.ShowMessage(fullHealthMessage);
                return; // Non raccoglie
            }

            player.Heal(healingPercentage);
            Destroy(gameObject);
        }
    }
}
