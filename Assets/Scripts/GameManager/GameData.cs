using UnityEngine;

[System.Serializable]
public class InventorySlotData
{
    public string itemName; // The name of the ItemSO (or empty string if slot is empty)
    public int quantity;    // The quantity of items in the slot
}
[System.Serializable]
public class GameData
{
    // Player position (from PlayerMovement)
    public Vector3 playerPosition;

    // Health stats (from StatsManager)
    public int currentHealth;
    public int maxHealth;

    // Experience and level (from ExpManager)
    public int currentExp;
    public int level;
    public int expToLevel;

    // Combat and movement stats (from StatsManager)
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    public int speed;

    // Equipment state (from Player_ChangeEquipment)
    public bool isUsingBow; // True if Player_Bow is enabled, false if Player_Combat is enabled

    public int gold;                    // Player's gold
    public InventorySlotData[] inventorySlots; // Array of inventory slots
}