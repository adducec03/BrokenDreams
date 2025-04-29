using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab; //Item that chest drops
    public Sprite openedSprite;
    public string requiredPassword = "1234";
    private PasswordPanel passwordPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        passwordPanel = FindFirstObjectByType<PasswordPanel>();
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);  
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        if (passwordPanel != null)
        {
            passwordPanel.OpenPasswordPrompt(this);
        }
    }

    public bool TryUnlock(string enteredPassword)
    {
        if (enteredPassword == requiredPassword)
        {
            OpenChest();
            return true;
        }
        return false;
    }

    private void OpenChest()
    {
        SetOpened(true);

        if(itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
        }
    }

    public void SetOpened(bool opened)
    {
        if(IsOpened = opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
