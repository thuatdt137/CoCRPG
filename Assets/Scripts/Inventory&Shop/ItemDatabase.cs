using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [SerializeField] private ItemSO[] allItems; // Assign all ItemSOs in the Inspector
    private Dictionary<string, ItemSO> itemMap; // Maps itemName to ItemSO

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Build the item map
        itemMap = new Dictionary<string, ItemSO>();
        foreach (var item in allItems)
        {
            if (!string.IsNullOrEmpty(item.itemName) && !itemMap.ContainsKey(item.itemName))
            {
                itemMap[item.itemName] = item;
            }
            else
            {
                Debug.LogWarning($"Duplicate or invalid item name: {item.itemName}");
            }
        }
    }

    public ItemSO GetItemByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return null;
        }

        if (itemMap.TryGetValue(itemName, out ItemSO item))
        {
            return item;
        }

        Debug.LogWarning($"Item not found in database: {itemName}");
        return null;
    }
}