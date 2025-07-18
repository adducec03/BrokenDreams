using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public List<InventorySaveData> inventorySaveData;
    public List<ChestSaveData> chestSaveData;
    public List<SceneItemSaveData> sceneItemsSaveData;
    public List<EnemySaveData> enemySaveData;
    public float playerHealth;
    public float playerShield;
    public int playerLives;
    public List<string> activatedPressurePads = new List<string>();
    public List<string> disabledWalls = new List<string>();
    public List<string> usedHealingPickups = new List<string>();
    public float maxHealth;
    public float maxShield;
    public bool isShieldActive;
    public int playerAttackDamage;

}
