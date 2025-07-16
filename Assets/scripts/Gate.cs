using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    [SerializeField] private BossAI bossAI;
    private MessageDisplay messageDisplay; // L'oggetto per far stampare dei log direttamente sullo schermo
    public string requiredItemID = "1"; // L'ID dell'oggetto chiave
    public Animator animator;             // Animatore per l'apertura, se presente
    public string openTrigger = "Open";   // Nome del trigger nell'Animator

    private bool isOpen = false;



    void Start()
    {
        messageDisplay = FindFirstObjectByType<MessageDisplay>();
    }

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
            messageDisplay.ShowMessage("Hai bisogno della chiave per aprire questo cancello.");
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
            animator.SetTrigger(openTrigger);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        messageDisplay.ShowMessage("Cancello aperto!");

        if (bossAI != null)
            bossAI.StartSummoning();
    }



}
