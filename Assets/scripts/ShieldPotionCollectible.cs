using UnityEngine;

public class ShieldPotionCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.EnableShield();
            GetComponent<CollectibleItem>().MarkCollected();
        }
    }
}
