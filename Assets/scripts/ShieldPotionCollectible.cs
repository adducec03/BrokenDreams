using UnityEngine;

public class ShieldPotionCollectible : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (FindFirstObjectByType<SaveController>().IsItemCollected(GetComponent<CollectibleItem>().itemID)) return;

        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            collected = true;
            player.EnableShield();

            GetComponent<CollectibleItem>().MarkCollected();
        }
    }
}
