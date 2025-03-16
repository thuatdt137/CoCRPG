using UnityEngine;
using UnityEngine.Diagnostics;

public class SkillManager : MonoBehaviour
{

    public Player_Combat combat;
    void OnEnable()
    {
        SkillSlots.OnAbilityPointSpent += HandleAbilityPointSpent;
    }
    void OnDisable()
    {
        SkillSlots.OnAbilityPointSpent -= HandleAbilityPointSpent;

    }

    private void HandleAbilityPointSpent(SkillSlots slot)
    {
        string skillName = slot.skillSO.skillName;

        switch (skillName)
        {
            case "Max Health Boost":
                StatsManager.Instance.UpdateMaxHealth(1);
                break;
            case "Sword Slash":
                combat.enabled = true;
                break;
            default:
                Debug.LogWarning("Unknown skill: " + skillName);
                break;
        }
    }
}
