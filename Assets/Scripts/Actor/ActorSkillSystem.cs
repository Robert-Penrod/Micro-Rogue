using System.Collections.Generic;
using UnityEngine;

public class ActorSkillSystem : MonoBehaviour
{
    Actor _actor;
    List<Skill> _skillList = new();

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        RefreshSkillList();
    }

    void RefreshSkillList()
    {
        _skillList.Clear();
        _skillList.AddRange(GetComponentsInChildren<Skill>());
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
