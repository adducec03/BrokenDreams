using UnityEngine;
using UnityEngine.EventSystems;

public class ReadableItem : Item, IInteractable, IPointerClickHandler
{
    [TextArea]
    public string testoPergamena;

    public void Interact()
    {
        UIManager.Instance.ShowText(testoPergamena);
        SoundEffectManager.Play("PageSound");
    }

    public bool CanInteract()
    {
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.ShowText(testoPergamena);
        SoundEffectManager.Play("PageSound");
    }
}
