using UnityEngine;

public class PlayerSaveHandler : MonoBehaviour
{
    // References to the player's components
    private PlayerMovement playerMovement;
    private ExpManager expManager;
    private Player_ChangeEquipment changeEquipment;
    private Player_Combat playerCombat;
    private Player_Bow playerBow;

    // Reference to the SaveSystem
    private SaveSystem saveSystem;
    private InventoryManager inventoryManager;

    void Awake()
    {
        // Get references to the player's components
        playerMovement = GetComponent<PlayerMovement>();
        expManager = FindObjectOfType<ExpManager>();
        changeEquipment = GetComponent<Player_ChangeEquipment>();
        playerCombat = GetComponent<Player_Combat>();
        playerBow = GetComponent<Player_Bow>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        // Find the SaveSystem in the scene
        saveSystem = FindObjectOfType<SaveSystem>();
    }

    void Update()
    {
        // Press S to save the game
        if (Input.GetKeyDown(KeyCode.I))
        {
            SavePlayerData();
        }

        // Press L to load the game
        if (Input.GetKeyDown(KeyCode.O))
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        GameData data = new GameData();

        // Save position (from PlayerMovement)
        data.playerPosition = transform.position;

        // Save health stats (from StatsManager)
        data.currentHealth = StatsManager.Instance.currentHealth;
        data.maxHealth = StatsManager.Instance.maxHealth;

        // Save experience and level (from ExpManager)
        data.currentExp = expManager.currentExp;
        data.level = expManager.level;
        data.expToLevel = expManager.expToLevel;

        // Save combat and movement stats (from StatsManager)
        data.damage = StatsManager.Instance.damage;
        data.weaponRange = StatsManager.Instance.weaponRange;
        data.knockbackForce = StatsManager.Instance.knockbackForce;
        data.knockbackTime = StatsManager.Instance.knockbackTime;
        data.stunTime = StatsManager.Instance.stunTime;
        data.speed = StatsManager.Instance.speed;

        // Save equipment state (from Player_ChangeEquipment)
        data.isUsingBow = playerBow.enabled;

        // Save inventory slots (from InventoryManager)
        data.inventorySlots = new InventorySlotData[inventoryManager.itemSlots.Length];
        for (int i = 0; i < inventoryManager.itemSlots.Length; i++)
        {
            InventorySlot slot = inventoryManager.itemSlots[i];
            data.inventorySlots[i] = new InventorySlotData
            {
                itemName = slot.itemSO != null ? slot.itemSO.itemName : "",
                quantity = slot.quantity
            };
        }

        // Save gold (from InventoryManager)
        data.gold = inventoryManager.gold;

        // Save the data using SaveSystem
        saveSystem.SaveGame(data);
    }

    public void LoadPlayerData()
    {
        GameData data = saveSystem.LoadGame();
        if (data != null)
        {
            // Load position (apply to PlayerMovement)
            transform.position = data.playerPosition;

            // Load health stats (apply to StatsManager)
            StatsManager.Instance.currentHealth = data.currentHealth;
            StatsManager.Instance.maxHealth = data.maxHealth;
            StatsManager.Instance.UpdateHealth(0); // Update UI

            // Load experience and level (apply to ExpManager)
            expManager.currentExp = data.currentExp;
            expManager.level = data.level;
            expManager.expToLevel = data.expToLevel;
            expManager.UpdateUI(); // Update UI

            // Load combat and movement stats (apply to StatsManager)
            StatsManager.Instance.damage = data.damage;
            StatsManager.Instance.weaponRange = data.weaponRange;
            StatsManager.Instance.knockbackForce = data.knockbackForce;
            StatsManager.Instance.knockbackTime = data.knockbackTime;
            StatsManager.Instance.stunTime = data.stunTime;
            StatsManager.Instance.speed = data.speed;
            StatsManager.Instance.statsUI.UpdateAllStats(); // Update UI

            // Load equipment state (apply to Player_ChangeEquipment)
            playerCombat.enabled = !data.isUsingBow;
            playerBow.enabled = data.isUsingBow;
            changeEquipment.transform.localScale = data.isUsingBow ? new Vector3(1f, 1f, 0) : new Vector3(1.5f, 1.5f, 0);

            // Load inventory slots (apply to InventoryManager)
            if (data.inventorySlots != null && data.inventorySlots.Length == inventoryManager.itemSlots.Length)
            {
                for (int i = 0; i < data.inventorySlots.Length; i++)
                {
                    InventorySlot slot = inventoryManager.itemSlots[i];
                    InventorySlotData slotData = data.inventorySlots[i];

                    // Look up the ItemSO by name
                    slot.itemSO = ItemDatabase.Instance.GetItemByName(slotData.itemName);
                    if (slot.itemSO == null && !string.IsNullOrEmpty(slotData.itemName))
                    {
                        Debug.LogWarning($"Could not find ItemSO for itemName: {slotData.itemName}. Slot {i} will be empty.");
                    }
                    slot.quantity = slotData.quantity;
                    slot.UpdateUI();
                }
            }
            else
            {
                Debug.LogWarning("Inventory slot data is missing or doesn't match the current inventory size. Clearing inventory.");
                foreach (var slot in inventoryManager.itemSlots)
                {
                    slot.itemSO = null;
                    slot.quantity = 0;
                    slot.UpdateUI();
                }
            }

            // Load gold (apply to InventoryManager)
            inventoryManager.gold = data.gold;
            inventoryManager.goldText.text = inventoryManager.gold.ToString();
        }
        else
        {
            Debug.Log("No save data found. Starting with default values.");
        }
    }
}