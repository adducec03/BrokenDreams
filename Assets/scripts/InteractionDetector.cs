using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public PasswordPanel passwordPanel;

    [Header("Distanza massima di interazione")]
    public float interactionRange = 2f;
    public Transform playerTransform;

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Blocca l'interazione se il menù è attivo
        if (menuController.isMenuOpen) return;

        if (context.performed && passwordPanel != null && !passwordPanel.IsPanelActive())
        {
            // Ottieni posizione del tocco/click sullo schermo
            Vector2 screenPos = Touchscreen.current != null
                ? Touchscreen.current.primaryTouch.position.ReadValue()
                : Mouse.current.position.ReadValue();

            // Converti in coordinate mondo
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // Fai raycast o overlap check
            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            if (hit != null && hit.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
            {
                float distance = Vector2.Distance(playerTransform.position, hit.transform.position);
                if (distance <= interactionRange)
                { 
                    interactable.Interact();
                }
                
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
    }
}
