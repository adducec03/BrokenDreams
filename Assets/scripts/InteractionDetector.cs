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
        if (context.performed && passwordPanel != null && !passwordPanel.IsPanelActive())
        {
            interactableInRange?.Interact();
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
