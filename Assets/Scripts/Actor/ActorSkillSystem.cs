using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorSkillSystem : MonoBehaviour
{
    [SerializeField] Transform _skillHolder;

    Actor _actor;
    public List<Skill> SkillList { get; private set; }

    public List<Skill> ActiveSkillList { get; private set; }
    public List<Skill> PassiveSkillList { get; private set; }

    private void Awake()
    {
        SkillList = new();
        ActiveSkillList = new();
        PassiveSkillList = new();
        _actor = GetComponentInParent<Actor>();
        RefreshSkillList();
    }

    public bool HasSkill(Skill skillToCheck)
    {
        return SkillList.Find(skill => skill.Name == skillToCheck.Name);
    }

    public Skill AddSkill(Skill skillPrefab)
    {
        var newSkill = Instantiate(skillPrefab, _skillHolder).GetComponent<Skill>();
        SkillList.Add(newSkill);
        if (newSkill.Slot == Skill.SlotEnum.Main || newSkill.Slot == Skill.SlotEnum.Offhand) ActiveSkillList.Add(newSkill);
        if (newSkill.Slot == Skill.SlotEnum.Passive) PassiveSkillList.Add(newSkill);
        return newSkill;
    }

    void RefreshSkillList()
    {
        SkillList.Clear();
        ActiveSkillList.Clear();
        PassiveSkillList.Clear();
        SkillList.AddRange(_skillHolder.GetComponentsInChildren<Skill>());
        ActiveSkillList.AddRange(SkillList.FindAll(x => x.Slot != Skill.SlotEnum.Passive));
        PassiveSkillList.AddRange(SkillList.FindAll(x => x.Slot == Skill.SlotEnum.Passive));
    }

    public bool ShouldAiChaseDown()
    {
        foreach(Skill skill in SkillList)
        {
            foreach(SkillInstance skillInstance in skill.SkillInstances)
            {
                // Damaging Skill Instance
                bool skillStarting = skillInstance.State == SkillInstance.SkillInstanceState.Start;
                bool skillDoesDamage = skillInstance.Skill.Stats.Damage.Value > 0;
                
                // Telegraph Skill
                if (skillInstance.State == SkillInstance.SkillInstanceState.Start && skillDoesDamage) return true;

                // Chasedown skill
                if (skillInstance.State != SkillInstance.SkillInstanceState.End && skillInstance.Skill.ChasedownWhileActive) return true;
            }
        }
        return false;
    }
}
