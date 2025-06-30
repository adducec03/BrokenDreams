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
    private List<EnemySaveState> enemies = new List<EnemySaveState>();
    private HashSet<string> activatedPressurePads = new HashSet<string>();


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
        enemies = FindObjectsByType<EnemySaveState>(FindObjectsSortMode.None).ToList();
    }

    public void RegisterSceneItem(CollectibleItem item)
    {
        if (!sceneItems.Any(i => i.itemID == item.itemID))
        {
            sceneItems.Add(item);
        }
    }

    public void SaveGame()
    {
        PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        SaveData saveData = new SaveData
        {
            playerPosition = playerStats.transform.position,
            inventorySaveData = inventoryController.GetInventoryItems(),
            chestSaveData = GetChestsState(),
            sceneItemsSaveData = GetSceneItemsState(),
            enemySaveData = GetEnemiesState(),
            playerHealth = playerStats.currentHealth,
            playerShield = playerStats.currentShield,
            playerLives = playerStats.lives,
            activatedPressurePads = activatedPressurePads.ToList()
        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestsState()
    {
        return chests.Select(chest => new ChestSaveData
        {
            chestID = chest.ChestID,
            isOpened = chest.IsOpened
        }).ToList();
    }

    private List<SceneItemSaveData> GetSceneItemsState()
    {
        return sceneItems.Select(item => item.GetSaveData()).ToList();
    }

    private List<EnemySaveData> GetEnemiesState()
    {
        return enemies.Select(enemy => enemy.GetSaveData()).ToList();
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            activatedPressurePads = new HashSet<string>(saveData.activatedPressurePads);
            PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            

            playerStats.transform.position = saveData.playerPosition;
            Time.timeScale = 1f;

            // Carica salute/scudo/vite
            playerStats.currentHealth = saveData.playerHealth;
            playerStats.currentShield = saveData.playerShield;
            playerStats.lives = saveData.playerLives;

            // Aggiorna UI
            playerStats.healthBarGame.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
            playerStats.shieldBarGame.SetShield(playerStats.currentShield, playerStats.maxShield);
            playerStats.healthBarMenu.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
            playerStats.shieldBarMenu.SetShield(playerStats.currentShield, playerStats.maxShield);
            playerStats.heartsManager.UpdateHearts(playerStats.lives);

            // Carica gli altri sistemi
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            LoadChestStates(saveData.chestSaveData);
            LoadSceneItemsState(saveData.sceneItemsSaveData);
            LoadEnemiesState(saveData.enemySaveData);
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
            ChestSaveData data = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if (data != null)
            {
                chest.SetOpened(data.isOpened);
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

    private void LoadEnemiesState(List<EnemySaveData> savedEnemies)
    {
        foreach (EnemySaveState enemy in enemies)
        {
            EnemySaveData data = savedEnemies.FirstOrDefault(e => e.enemyID == enemy.enemyID);
            if (data != null)
            {
                enemy.LoadFromSave(data);
            }
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
        }
    }

    public bool IsPressurePadActivated(string id)
    {
        return activatedPressurePads.Contains(id);
    }

    public void SetPressurePadActivated(string id, bool state)
    {
        if (string.IsNullOrWhiteSpace(id)) return;

        if (state)
            activatedPressurePads.Add(id);
        else
            activatedPressurePads.Remove(id);
    }

}
