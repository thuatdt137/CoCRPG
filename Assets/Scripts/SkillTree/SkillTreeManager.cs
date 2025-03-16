using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public SkillSlots[] skillSlots;
    public TMP_Text pointsText;
    public int availablePoints;

    void OnEnable()
    {
        SkillSlots.OnAbilityPointSpent += HandleAbilityPointsSpent;
        SkillSlots.OnSkillMaxed += HandleSkillMaxed;
        ExpManager.OnLevelUp += UpdateAbilityPoints;
    }
    void OnDisable()
    {
        SkillSlots.OnAbilityPointSpent -= HandleAbilityPointsSpent;
        SkillSlots.OnSkillMaxed -= HandleSkillMaxed;
        ExpManager.OnLevelUp -= UpdateAbilityPoints;
    }

    void Start()
    {
        foreach (SkillSlots slot in skillSlots)
        {
            slot.skillButton.onClick.AddListener(() => CheckAvailablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }

    private void CheckAvailablePoints(SkillSlots slot)
    {
        if (availablePoints > 0)
        {
            slot.TryUpgradeSkill();
        }
    }

    private void HandleAbilityPointsSpent(SkillSlots skillSlots)
    {
        if (availablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
    }

    private void HandleSkillMaxed(SkillSlots skillSlot)
    {
        foreach (SkillSlots slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlock();
        }
    }


    public void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        pointsText.text = "Points: " + availablePoints;
    }
}
