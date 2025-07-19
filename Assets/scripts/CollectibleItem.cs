using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemID;
    private bool isCollected = false;

    void Start()
    {
        SaveController saveController = FindFirstObjectByType<SaveController>();
        if (saveController != null)
        {
            // Se è stato già raccolto, disattivalo immediatamente
            if (saveController.IsItemCollected(itemID))
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void MarkCollected()
    {
        isCollected = true;
        gameObject.SetActive(false);
        SoundEffectManager.Play("PageSound");
    }

    public SceneItemSaveData GetSaveData()
    {
        return new SceneItemSaveData
        {
            itemID = itemID,
            isCollected = isCollected
        };
    }

    public void LoadFromSave(SceneItemSaveData data)
    {
        isCollected = data.isCollected;
        gameObject.SetActive(!isCollected);
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = GlobalHelper.GenerateUniqueID(gameObject);
        }

        SaveController saveController = FindFirstObjectByType<SaveController>();
        if (saveController != null)
        {
            saveController.RegisterSceneItem(this);
            if (saveController.IsItemCollected(itemID))
            {
                isCollected = true;
                gameObject.SetActive(false);
            }
        }
    }
}
