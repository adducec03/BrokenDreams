using Firebase.Firestore;

[FirestoreData]
public class EnemySaveData
{
    [FirestoreProperty] public string enemyID { get; set; }
    [FirestoreProperty] public bool isDefeated { get; set; }
}
