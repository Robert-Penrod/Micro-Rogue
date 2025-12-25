using System.Collections.Generic;
using UnityEngine;

public class ActorSkillSystem : MonoBehaviour
{
    [SerializeField] Transform _skillHolder;

    Actor _actor;
    public List<Skill> SkillList { get; private set; }

    private void Awake()
    {
        SkillList = new();
        _actor = GetComponentInParent<Actor>();
        RefreshSkillList();
    }

    public void AddSkill(Skill skillPrefab)
    {
        var newSkill = Instantiate(skillPrefab, _skillHolder).GetComponent<Skill>();
        SkillList.Add(newSkill);
    }

    void RefreshSkillList()
    {
        SkillList.Clear();
        SkillList.AddRange(_skillHolder.GetComponentsInChildren<Skill>());
    }

    public bool IsAttacking()
    {
        foreach(Skill skill in SkillList)
        {
            foreach(SkillInstance skillInstance in skill.SkillInstances)
            {
                // Damaging Skill Instance
                bool skillStarting = skillInstance.State == SkillInstance.SkillInstanceState.Start;
                bool skillDoesDamage = skillInstance.Skill.Stats.Damage.Value > 0;
                
                if (skillStarting && skillDoesDamage)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
