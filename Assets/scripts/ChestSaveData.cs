using Firebase.Firestore;

[FirestoreData]
public class ChestSaveData
{
    [FirestoreProperty] public string chestID { get; set; }
    [FirestoreProperty] public bool isOpened { get; set; }
}
