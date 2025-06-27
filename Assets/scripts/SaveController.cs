using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private Chest[] chests;
    private List<CollectibleItem> sceneItems = new List<CollectibleItem>();

    void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindFirstObjectByType<InventoryController>();
        chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
        sceneItems = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).ToList();
    }

    public void RegisterSceneItem(CollectibleItem item)
    {
        if (!sceneItems.Any(i => i.itemID == item.itemID))
        {
            sceneItems.Add(item);
            Debug.Log("Item registrato: " + item.itemID);
        }
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = inventoryController.GetInventoryItems(),
            chestSaveData = GetChestsState(),
            sceneItemsSaveData = GetSceneItemsState()
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();
        foreach (Chest chest in chests)
        {
            chestStates.Add(new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.IsOpened
            });
        }
        return chestStates;
    }

    private List<SceneItemSaveData> GetSceneItemsState()
    {
        return sceneItems.Select(item => item.GetSaveData()).ToList();
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            Time.timeScale = 1f;
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            LoadChestStates(saveData.chestSaveData);
            LoadSceneItemsState(saveData.sceneItemsSaveData);
        }
        else
        {
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            SaveGame();
        }
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }

    private void LoadSceneItemsState(List<SceneItemSaveData> savedItems)
    {
        foreach (CollectibleItem item in sceneItems)
        {
            SceneItemSaveData data = savedItems.FirstOrDefault(i => i.itemID == item.itemID);
            if (data != null)
            {
                item.LoadFromSave(data);
            }
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log("Salvataggio eliminato.");
        }
    }

}
