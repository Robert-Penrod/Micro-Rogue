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
    public List<Skill> ItemList { get; private set; }

    public Action<Skill, Actor> OnSkillAdded;

    private void Awake()
    {
        SkillList = new();
        ActiveSkillList = new();
        PassiveSkillList = new();
        ItemList = new();
        _actor = GetComponentInParent<Actor>();
        RefreshSkillList();
    }

    private void Start()
    {
        // Load Player Skills
        this.DelayedInvoke(-1, () =>
        {
            if (_actor.IsPlayer())
            {
                var player = _actor.GetComponentInParent<Player>();
                if (player != null)
                {
                    int playerNum = 1 + PlayerManager.I.PlayerList.IndexOf(player);
                    var skillString = PlayerPrefs.GetString(GetSkillStringKey(playerNum), string.Empty);
                    LoadSkillString(skillString);
                }
            }
        });
    }

    public bool HasSkill(Skill skillToCheck)
    {
        return SkillList.Find(skill => skill != null && skill.Name == skillToCheck.Name);
    }

    public Skill AddSkill(Skill skillPrefab)
    {
        var newSkill = Instantiate(skillPrefab, _skillHolder).GetComponent<Skill>();
        SkillList.Add(newSkill);
        if (newSkill.Slot == Skill.SlotEnum.Main || newSkill.Slot == Skill.SlotEnum.Offhand) ActiveSkillList.Add(newSkill);
        if (newSkill.Slot == Skill.SlotEnum.Passive) PassiveSkillList.Add(newSkill);
        if (newSkill.Slot == Skill.SlotEnum.Item) ItemList.Add(newSkill);

        RefreshSkillList();

        // Save Player Skill String On Add
        if(_actor.IsPlayer())
        {
            var player = _actor.GetComponentInParent<Player>();
            if (player != null)
            {
                int playerNum = 1 + PlayerManager.I.PlayerList.IndexOf(player);
                string skillString = !DungeonManager.I.IsRunStarted ? GetSkillString() : string.Empty;
                PlayerPrefs.SetString(GetSkillStringKey(playerNum), skillString);
            }
        }

        OnSkillAdded?.Invoke(newSkill, _actor);

        return newSkill;
    }
    public string GetSkillStringKey(int playerNum)
    {
        return $"PlayerPrepSkillString_{playerNum}";
    }
    public void LoadSkillString(string skillString)
    {
        RemoveAllSkills();
        var globalSkillList = UpgradeManager.I.GetSkillList();
        string[] skillNames = skillString.Split(",");
        foreach(var name in skillNames)
        {
            var skill = globalSkillList.Find(x => x.Name == name);
            if (skill == null) continue;
            AddSkill(skill);
        }
    }

    public string GetSkillString()
    {
        string skillString = string.Empty;
        foreach(var skill in SkillList)
        {
            if (skillString.Length > 0) skillString += ",";
            skillString += skill.Name;
        }
        return skillString;
    }

    void RefreshSkillList()
    {
        SkillList.Clear();
        ActiveSkillList.Clear();
        PassiveSkillList.Clear();
        ItemList.Clear();
        SkillList.AddRange(_skillHolder.GetComponentsInChildren<Skill>());
        ActiveSkillList.AddRange(SkillList.FindAll(x => x.Slot == Skill.SlotEnum.Main || x.Slot == Skill.SlotEnum.Offhand));
        PassiveSkillList.AddRange(SkillList.FindAll(x => x.Slot == Skill.SlotEnum.Passive));
        ItemList.AddRange(SkillList.FindAll(x => x.Slot == Skill.SlotEnum.Item));
    }

    public void RemoveAllSkills()
    {
        foreach (Transform skill in this.transform)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(skill.gameObject);
            }
            else
            {
                Destroy(skill.gameObject);
            }
        }
        if (SkillList != null)
        {
            SkillList.Clear();
            ActiveSkillList.Clear();
            PassiveSkillList.Clear();
            ItemList.Clear();
        }
    }

    #region AI
    public float GetAI_AttackChase()
    {
        float chase = 0f;
        int count = 0;
        float totalLevels = 0f;
        foreach (Skill skill in SkillList)
        {
            foreach (SkillInstance skillInstance in skill.SkillInstances)
            {
                // get info
                bool skillStarting = skillInstance.State == SkillInstance.SkillInstanceState.Start;
                bool skillDoesDamage = skillInstance.Skill.Stats.Damage.Value > 0;
                // Telegraph Skill
                if (skillInstance.State == SkillInstance.SkillInstanceState.Start && skillDoesDamage) ChaseMult(skillInstance.Skill);
                // Chasedown skill
                else if (skillInstance.State != SkillInstance.SkillInstanceState.End) ChaseMult(skillInstance.Skill);
            }
            
        }
        void ChaseMult(Skill skill)
        {
            var level = skill.Level == 0 ? 1 : skill.Level;
            chase += skill.AttackChase * level;
            totalLevels += level;
            count++;
        }
        if (count == 0) chase = 0f;
        else chase /= (float)totalLevels; // count
        return chase;
    }

    public bool GetAI_IsAttacking()
    {
        foreach (Skill skill in SkillList)
        {
            foreach (SkillInstance skillInstance in skill.SkillInstances)
            {
                // get info
                bool skillStarting = skillInstance.State == SkillInstance.SkillInstanceState.Start;
                bool skillDoesDamage = skillInstance.Skill.Stats.Damage.Value > 0;
                // Telegraph Skill
                if (skillInstance.State == SkillInstance.SkillInstanceState.Start && skillDoesDamage) return true;
                // Chasedown skill
                else if (skillInstance.State != SkillInstance.SkillInstanceState.End) return true;
            }
        }
        return false;
    }

    public float GetAI_PassiveDistMult()
    {
        float passiveDistMult = 0f;
        foreach (Skill skill in SkillList)
        {
            passiveDistMult += skill.PassiveDistMult;
        }
        return SkillList.Count == 0 ? 1f : passiveDistMult /= SkillList.Count;
    }
    #endregion
}
