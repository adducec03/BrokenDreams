using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
public class SaveData
{
    [FirestoreProperty] public float playerX { get; set; }
    [FirestoreProperty] public float playerY { get; set; }
    [FirestoreProperty] public float playerZ { get; set; }

    [FirestoreProperty] public float playerHealth { get; set; }
    [FirestoreProperty] public float playerShield { get; set; }
    [FirestoreProperty] public int playerLives { get; set; }

    [FirestoreProperty] public float maxHealth { get; set; }
    [FirestoreProperty] public float maxShield { get; set; }

    [FirestoreProperty] public bool isShieldActive { get; set; }
    [FirestoreProperty] public int playerAttackDamage { get; set; }

    [FirestoreProperty] public bool isBossFightStarted { get; set; }
    [FirestoreProperty] public bool isBossDead { get; set; }
    [FirestoreProperty] public int bossCurrentHealth { get; set; }
    [FirestoreProperty] public bool isBossGateLocked { get; set; }

    [FirestoreProperty] public List<string> activatedPressurePads { get; set; }
    [FirestoreProperty] public List<string> disabledWalls { get; set; }
    [FirestoreProperty] public List<string> usedHealingPickups { get; set; }

    [FirestoreProperty] public List<InventorySaveData> inventory { get; set; }
    [FirestoreProperty] public List<ChestSaveData> chests { get; set; }
    [FirestoreProperty] public List<SceneItemSaveData> sceneItems { get; set; }
    [FirestoreProperty] public List<EnemySaveData> enemies { get; set; }
}
