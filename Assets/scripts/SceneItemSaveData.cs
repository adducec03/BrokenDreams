using Firebase.Firestore;

[FirestoreData]
public class SceneItemSaveData
{
    [FirestoreProperty] public string itemID { get; set; }
    [FirestoreProperty] public bool isCollected { get; set; }
}
