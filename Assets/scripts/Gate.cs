using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    public string requiredItemID = "1"; // L'ID dell'oggetto chiave
    public Animator animator;             // Animatore per l'apertura, se presente
    public string openTrigger = "Open";   // Nome del trigger nell'Animator

    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen) return;

        InventoryController inventory = FindFirstObjectByType<InventoryController>();

        if (inventory != null && inventory.HasItem(requiredItemID))
        {
            OpenGate();
        }
        else
        {
            Debug.Log("Hai bisogno della chiave per aprire questo cancello.");
        }
    }

    public bool CanInteract()
    {
        // Permetti l'interazione solo se il cancello è ancora chiuso
        return !isOpen;
    }

    private void OpenGate()
    {
        isOpen = true;

        if (animator != null)
        {
            animator.SetTrigger(openTrigger);
        }

        // Disattiva il collider così il player può passare
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Debug.Log("Cancello aperto!");
    }

}
