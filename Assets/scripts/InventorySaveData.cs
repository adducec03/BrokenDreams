using Firebase.Firestore;

[FirestoreData]
public class InventorySaveData
{
    [FirestoreProperty] public int itemID { get; set; }
    [FirestoreProperty] public int slotIndex { get; set; }
}
