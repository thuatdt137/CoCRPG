using UnityEngine;

public class Enemy_Health : MonoBehaviour
{

    public int expReward = 3;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;
    public int currentHealth;
    public int maxHealth;
    public GameObject itemPrefab;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            OnMonsterDefeated(expReward);
            DropItem();
            Destroy(gameObject);
        }
    }


    void DropItem()
    {
        if (itemPrefab != null)
        {
            GameObject itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
