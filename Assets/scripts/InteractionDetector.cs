using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public PasswordPanel passwordPanel;

    public void OnInteract(InputAction.CallbackContext context)
    {
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
                interactable.Interact();
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
