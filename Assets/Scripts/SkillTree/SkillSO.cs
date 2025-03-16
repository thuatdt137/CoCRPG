
using UnityEngine;


[CreateAssetMenu(fileName = "NewSkill", menuName = "SKillTree/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public int maxLevel;
    public Sprite skillIcon;
}