using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Firebase.Extensions;


public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private Chest[] chests;
    private List<CollectibleItem> sceneItems = new List<CollectibleItem>();
    private List<EnemySaveState> enemies = new List<EnemySaveState>();
    private HashSet<string> activatedPressurePads = new HashSet<string>();
    private HashSet<string> disabledWallIDs = new HashSet<string>();
    private HashSet<string> usedHealingPickups = new HashSet<string>();
    public bool isDataLoaded { get; private set; } = false;
    private HashSet<string> collectedItemIDs = new HashSet<string>();

    void Start()
    {
        InitializeComponents();

        if (FirebaseManager.Instance != null && FirebaseManager.Instance.GetUserID() != null)
            LoadGameFromFirestore();  // Carica da Firestore
        else
            LoadGame();               // Fallback locale
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, $"save_{FirebaseManager.Instance.GetUsername()}.json");
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
        BossAI boss = GameObject.FindFirstObjectByType<BossAI>();
        Gate gate = FindFirstObjectByType<Gate>();

        Vector3 pos = playerStats.transform.position;

        SaveData saveData = new SaveData
        {
            playerX = pos.x,
            playerY = pos.y,
            playerZ = pos.z,
            inventory = inventoryController.GetInventoryItems(),
            chests = GetChestsState(),
            sceneItems = GetSceneItemsState(),
            enemies = GetEnemiesState(),

            playerHealth = playerStats.currentHealth,
            playerShield = playerStats.currentShield,
            playerLives = playerStats.lives,
            maxHealth = playerStats.maxHealth,
            maxShield = playerStats.maxShield,
            isShieldActive = playerStats.HasShield(),
            playerAttackDamage = playerStats.GetComponent<PlayerAttack>().attackDamage,

            activatedPressurePads = activatedPressurePads.ToList(),
            disabledWalls = disabledWallIDs.ToList(),
            usedHealingPickups = usedHealingPickups.ToList(),

            isBossFightStarted = boss != null && boss.IsFightStarted(),
            isBossDead = boss == null || boss.IsDead(),
            bossCurrentHealth = boss != null ? boss.GetCurrentHealth() : 0,
            isBossGateLocked = gate != null && gate.IsLockedForever()
        };

        string userId = FirebaseManager.Instance.GetUserID();

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(userId)
            .Collection("data")
            .Document("saveData")
            .SetAsync(saveData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log("Salvataggio completato su Firestore");
                else
                    Debug.LogError("Errore salvataggio Firestore: " + task.Exception);
            });
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
            LoadSaveDataIntoGame(saveData);
        }
        else
        {
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            SaveGame();
        }
    }

    private void LoadSaveDataIntoGame(SaveData saveData)
    {
        activatedPressurePads = new HashSet<string>(saveData.activatedPressurePads);
        disabledWallIDs = new HashSet<string>(saveData.disabledWalls);

        PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerStats.transform.position = new Vector3(saveData.playerX, saveData.playerY, saveData.playerZ);
        Time.timeScale = 1f;

        playerStats.maxHealth = saveData.maxHealth;
        playerStats.maxShield = saveData.maxShield;
        playerStats.currentHealth = saveData.playerHealth;
        playerStats.currentShield = saveData.playerShield;
        playerStats.lives = saveData.playerLives;
        playerStats.GetComponent<PlayerAttack>().attackDamage = saveData.playerAttackDamage;

        if (saveData.isShieldActive)
        {
            playerStats.shieldBarGame.gameObject.SetActive(true);
            playerStats.shieldBarMenu.gameObject.SetActive(true);
            if (playerStats.auraObject != null) playerStats.auraObject.SetActive(true);
            if (playerStats.auraObjectUI != null) playerStats.auraObjectUI.SetActive(true);
            playerStats.EnableAuraSound();
            playerStats.SetShieldState(true);
        }

        usedHealingPickups = new HashSet<string>(saveData.usedHealingPickups);

        collectedItemIDs = new HashSet<string>(
            saveData.sceneItems
                .Where(i => i.isCollected)
                .Select(i => i.itemID)
        );

        playerStats.healthBarGame.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        playerStats.shieldBarGame.SetShield(playerStats.currentShield, playerStats.maxShield);
        playerStats.healthBarMenu.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        playerStats.shieldBarMenu.SetShield(playerStats.currentShield, playerStats.maxShield);
        playerStats.heartsManager.UpdateHearts(playerStats.lives);

        BossAI boss = FindFirstObjectByType<BossAI>();
        if (boss != null)
        {
            if (saveData.isBossDead)
            {
                Destroy(boss.gameObject);
            }
            else
            {
                boss.SetCurrentHealth(saveData.bossCurrentHealth);
                if (saveData.isBossFightStarted)
                {
                    boss.healthBarUI.SetActive(true);
                    boss.StartSummoning();
                }
            }
        }

        Gate gate = FindFirstObjectByType<Gate>();
        if (gate != null && saveData.isBossGateLocked)
        {
            gate.LockForever();
        }

        LoadSceneItemsState(saveData.sceneItems);
        inventoryController.SetInventoryItems(saveData.inventory);
        LoadChestStates(saveData.chests);
        LoadEnemiesState(saveData.enemies);

        isDataLoaded = true;
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

    public void SetWallDisabled(string id, bool state)
    {
        if (string.IsNullOrWhiteSpace(id)) return;

        if (state)
            disabledWallIDs.Add(id);
        else
            disabledWallIDs.Remove(id);
    }

    public bool IsWallDisabled(string id)
    {
        return disabledWallIDs.Contains(id);
    }

    public bool IsHealingUsed(string id)
    {
        return usedHealingPickups.Contains(id);
    }

    public void SetHealingUsed(string id, bool state)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.Log("fallito");
            return;
        }
        usedHealingPickups.Add(id);
    }

    public bool IsItemCollected(string id)
    {
        return collectedItemIDs.Contains(id);
    }

    public void LoadGameFromFirestore()
    {
        string userId = FirebaseManager.Instance.GetUserID();

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(userId)
            .Collection("data")
            .Document("saveData")
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    SaveData saveData = task.Result.ConvertTo<SaveData>();
                    LoadSaveDataIntoGame(saveData);
                }
                else
                {
                    Debug.Log("Nessun salvataggio trovato su Firestore. Creo uno nuovo.");
                    SaveGame();
                }
            });
    }
}
