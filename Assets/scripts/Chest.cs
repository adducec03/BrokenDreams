using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;
    public string requiredPassword = "";
    private PasswordPanel passwordPanel;

    [TextArea] public string passwordPlaceholder = "";
    [SerializeField] private string optionalMessage = "";

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

        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);

            CollectibleItem collectible = droppedItem.GetComponent<CollectibleItem>();
            if (collectible == null)
            {
                collectible = droppedItem.AddComponent<CollectibleItem>();
            }

            collectible.itemID = ChestID + "_drop";

            SaveController saveController = FindFirstObjectByType<SaveController>();
            if (saveController != null)
            {
                saveController.RegisterSceneItem(collectible);
            }

            droppedItem.tag = "Item";            
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }

    public string GetOptionalMessage()
    {
        return optionalMessage;
    }
}
