using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;
    private bool inventoryAlreadyLoaded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        // for (int i = 0; i < slotCount; i++)
        // {
        //     Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
        //     if (i < itemPrefabs.Length)
        //     {
        //         GameObject item = Instantiate(itemPrefabs[i], slot.transform);
        //         item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //         slot.currentItem = item;
        //     }
        // }
    }

    public bool AddItem(GameObject itemPrefab, bool fromLoad = false)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;

                Item itemComponent = newItem.GetComponent<Item>();
                if (!fromLoad && itemComponent != null && itemComponent.ID == 2)
                {
                    Debug.Log("[Inventory] Pozione scudo aggiunta. Attivazione scudo...");
                    PlayerStats player = FindFirstObjectByType<PlayerStats>();
                    if (player != null)
                    {
                        player.EnableShield();
                    }
                }

                return true;
            }
        }
        Debug.Log("Inventory is full!");
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {

        if (inventoryAlreadyLoaded)
        {
            Debug.LogWarning("[Inventory] Inventory già caricato, annullata doppia istanza.");
            return;
        }

        inventoryAlreadyLoaded = true;
        // Distruggi TUTTI gli slot esistenti
        foreach (Transform slot in inventoryPanel.transform)
        {
            Destroy(slot.gameObject);
        }

        // Ricrea slot da zero
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

                if (itemPrefab != null)
                {
                    foreach (Transform child in slot.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    GameObject item = Instantiate(itemPrefab, slot.transform);

                    RectTransform rect = item.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.anchoredPosition = Vector2.zero;
                    }
                    else
                    {
                        Debug.LogError($"[InventoryController] Il prefab '{item.name}' non ha un RectTransform. Assicurati che sia un prefab UI!");
                    }

                    slot.currentItem = item;
                }
                else
                {
                    Debug.LogError($"[InventoryController] Nessun prefab trovato per itemID: {data.itemID}. Controlla il tuo ItemDictionary.");
                }
            }
            else
            {
                Debug.LogWarning($"[InventoryController] Indice slot {data.slotIndex} fuori dal range (slotCount: {slotCount}).");
            }
            Debug.Log($"[Inventory] Slot {data.slotIndex} → itemID {data.itemID}");
        }


    }

    public bool HasItem(string itemID)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.ID.ToString() == itemID)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
