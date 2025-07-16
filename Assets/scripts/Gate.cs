using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    [SerializeField] private BossAI bossAI;
    private MessageDisplay messageDisplay;
    public string requiredItemID = "1";
    public Animator animator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    private bool isOpen = false;
    private bool isLockedForever = false; // ✅ nuova variabile
    private Collider2D col;

    void Start()
    {
        messageDisplay = FindFirstObjectByType<MessageDisplay>();
        col = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        if (isOpen || isLockedForever) return; // ✅ blocca se è chiuso definitivamente

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
        return !isOpen && !isLockedForever;
    }

    private void OpenGate()
    {
        isOpen = true;
        SoundEffectManager.Play("OpenGate");

        if (animator != null)
            animator.SetTrigger(openTrigger);

        if (col != null)
            col.enabled = false;

        messageDisplay.ShowMessage("Cancello aperto!");

        // Avvia la chiusura automatica
        Invoke(nameof(CloseGate), 3f);

        if (bossAI != null)
            bossAI.StartSummoning();
    }

    private void CloseGate()
    {
        if (animator != null)
            animator.SetTrigger(closeTrigger);

        if (col != null)
            col.enabled = true;

        SoundEffectManager.Play("CloseGate");
        

        isOpen = false;

        // ❌ Rende il cancello definitivamente non interagibile
        isLockedForever = true;


    }
}