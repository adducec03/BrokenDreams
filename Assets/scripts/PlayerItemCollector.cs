using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Item"))
        {
            CollectibleItem collectible = collision.GetComponent<CollectibleItem>();

            // Controllo che l'oggetto con cui sto interagendo non sia già stato collezionato (serve per evitare il bug che involontariamente gli oggetti si duplicano nell'inventario)
            if (collectible != null && collectible.IsCollected()) return;

            // Colleziona l'Item con cui interagisce
            Item item = collision.GetComponent<Item>();
            if(item != null)
            {
                bool itemAdded = inventoryController.AddItem(collision.gameObject);

                if(itemAdded)
                {
                    collectible.MarkCollected();
                }
            }
        }
    }
}
