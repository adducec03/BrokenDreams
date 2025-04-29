using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Hai premuto E!");

            if(interactableInRange != null)
            {
                Debug.Log($"Interagisco con {interactableInRange}");
                interactableInRange.Interact();
            }
            else
            {
                Debug.Log("Nessun oggetto con cui interagire.");
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"OnTriggerEnter2D con {collision.gameObject.name}");

        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            Debug.Log("Trovato un oggetto interagibile!");
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
