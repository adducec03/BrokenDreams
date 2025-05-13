using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemID;
    private bool isCollected = false;

    public void MarkCollected()
    {
        isCollected = true;
        gameObject.SetActive(false);
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
        }
    }
}
