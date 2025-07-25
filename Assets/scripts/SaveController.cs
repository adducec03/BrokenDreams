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
    private HashSet<string> disabledWallIDs = new HashSet<string>();
    private HashSet<string> usedHealingPickups = new HashSet<string>();
    public bool isDataLoaded { get; private set; } = false;
    private HashSet<string> collectedItemIDs = new HashSet<string>();



    void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, $"save_{SessionManager.currentUsername}.json");
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
            maxHealth = playerStats.maxHealth,
            maxShield = playerStats.maxShield,
            isShieldActive = playerStats.HasShield(),
            playerAttackDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().attackDamage,
            activatedPressurePads = activatedPressurePads.ToList(),
            disabledWalls = disabledWallIDs.ToList(),
            usedHealingPickups = usedHealingPickups.ToList(),

            isBossFightStarted = boss != null && boss.IsFightStarted(),
            isBossDead = boss == null || boss.IsDead(),
            bossCurrentHealth = boss != null ? boss.GetCurrentHealth() : 0,
            isBossGateLocked = gate != null && gate.IsLockedForever()
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

            // Carica dati vari
            activatedPressurePads = new HashSet<string>(saveData.activatedPressurePads);
            disabledWallIDs = new HashSet<string>(saveData.disabledWalls);

            // Carica le statistiche del Player
            PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            playerStats.transform.position = saveData.playerPosition;
            Time.timeScale = 1f;

            playerStats.maxHealth = saveData.maxHealth;
            playerStats.maxShield = saveData.maxShield;
            playerStats.currentHealth = saveData.playerHealth;
            playerStats.currentShield = saveData.playerShield;
            playerStats.lives = saveData.playerLives;
            playerStats.GetComponent<PlayerAttack>().attackDamage = saveData.playerAttackDamage;

            // Attiva aura se necessario
            if (saveData.isShieldActive)
            {
                playerStats.shieldBarGame.gameObject.SetActive(true);
                playerStats.shieldBarMenu.gameObject.SetActive(true);
                if (playerStats.auraObject != null) playerStats.auraObject.SetActive(true);
                if (playerStats.auraObjectUI != null) playerStats.auraObjectUI.SetActive(true);
                playerStats.EnableAuraSound();
                playerStats.SetShieldState(true);
            }

            // Healing pickup usati
            usedHealingPickups = new HashSet<string>(saveData.usedHealingPickups);

            // CARICA collected item ID PRIMA di chiamare LoadSceneItemsState
            collectedItemIDs = new HashSet<string>(
                saveData.sceneItemsSaveData
                    .Where(i => i.isCollected)
                    .Select(i => i.itemID)
            );

            // UI
            playerStats.healthBarGame.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
            playerStats.shieldBarGame.SetShield(playerStats.currentShield, playerStats.maxShield);
            playerStats.healthBarMenu.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
            playerStats.shieldBarMenu.SetShield(playerStats.currentShield, playerStats.maxShield);
            playerStats.heartsManager.UpdateHearts(playerStats.lives);

            // Carica lo stato salvato del boss
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

            // Carica lo stato salvato del cancello
            Gate gate = FindFirstObjectByType<Gate>();
            if (gate != null && saveData.isBossGateLocked)
            {
                gate.LockForever();
            }

            // Altri sistemi
            LoadSceneItemsState(saveData.sceneItemsSaveData); // Usa collectedItemIDs!
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            LoadChestStates(saveData.chestSaveData);
            LoadEnemiesState(saveData.enemySaveData);

            isDataLoaded = true;
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

}
