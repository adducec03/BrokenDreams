using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour, IInteractable
{
    [SerializeField] private BossAI bossAI;
    private MessageDisplay messageDisplay;
    public string requiredItemID = "1";
    public Animator animator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    private bool isOpen = false;
    private bool isLockedForever = false;
    private Collider2D col;
    public Transform checkpointTargetPosition; // Dove vuoi spostare il checkpoint
    public GameObject checkpointObject;

    void Start()
    {
        messageDisplay = FindFirstObjectByType<MessageDisplay>();
        col = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        if (isOpen || isLockedForever) return;

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

        if (checkpointObject != null && checkpointTargetPosition != null)
        {
            checkpointObject.transform.position = checkpointTargetPosition.position;
            Debug.Log("Checkpoint aggiornato dopo l'apertura del cancello.");
        }

        // Avvia la chiusura automatica
        Invoke(nameof(CloseGate), 3f);

        if (bossAI != null)
            bossAI.StartSummoning();

        if (bossAI != null && bossAI.healthBarUI != null)
            StartCoroutine(ShowBossHealthBarWithDelay(bossAI));
    }

    private IEnumerator ShowBossHealthBarWithDelay(BossAI boss)
    {
        yield return new WaitForSeconds(1f);
        boss.healthBarUI.SetActive(true);
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