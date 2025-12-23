using System.Collections.Generic;
using UnityEngine;

public class ActorSkillSystem : MonoBehaviour
{
    [SerializeField] Transform _skillHolder;

    Actor _actor;
    List<Skill> _skillList = new();

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        RefreshSkillList();
    }

    public void AddSkill(Skill skillPrefab)
    {
        var newSkill = Instantiate(skillPrefab, _skillHolder).GetComponent<Skill>();
        _skillList.Add(newSkill);
    }

    void RefreshSkillList()
    {
        _skillList.Clear();
        _skillList.AddRange(_skillHolder.GetComponentsInChildren<Skill>());
    }

    public bool IsAttacking()
    {
        foreach(Skill skill in _skillList)
        {
            foreach(SkillInstance skillInstance in skill.SkillInstances)
            {
                // Damaging Skill Instance
                bool skillStarting = skillInstance.State == SkillInstance.SkillInstanceState.Start;
                bool skillDoesDamage = skillInstance.Skill.Stats.Damage > 0;
                
                if (skillStarting && skillDoesDamage)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
