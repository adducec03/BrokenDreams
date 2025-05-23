using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public List<InventorySaveData> inventorySaveData;
    public List<ChestSaveData> chestSaveData;
    public List<SceneItemSaveData> sceneItemsSaveData;
}
