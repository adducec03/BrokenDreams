using UnityEngine;

public class EnemySaveState : MonoBehaviour
{
    public string enemyID; // Deve essere univoco
    public bool isDefeated;

    public EnemySaveData GetSaveData()
    {
        return new EnemySaveData
        {
            enemyID = this.enemyID,
            isDefeated = this.isDefeated
        };
    }

    public void LoadFromSave(EnemySaveData data)
    {
        isDefeated = data.isDefeated;

        if (isDefeated)
        {
            gameObject.SetActive(false); // Oppure: disattiva AI/animazioni
        }
    }

    public void MarkAsDefeated()
    {
        isDefeated = true;
    }
}
