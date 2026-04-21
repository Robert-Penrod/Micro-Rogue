using Kryz.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillUpgrade : Upgrade
{
    public List<UpgradeMod> ModList = new();

    public override string GetSlot() => SourceSkill.Slot.ToString();

    public override string GetTitle() => $"{SourceSkill.Name} ← {_name}";

    public override Sprite GetIcon()
    {
        return SourceSkill.Icon;
    }

    public override Color GetColor()
    {
        if (SourceSkill.SkillColor != Color.clear) return SourceSkill.SkillColor;
        return GamePaletteManager.I.Palette.GetSkillColor(SourceSkill);
    }

    public override string GetDescription()
    {
        string description = string.Empty;// _description + "\n";
        ModList.Sort((x, y) => x.SkillStatName > y.SkillStatName ? 1 : -1);
        foreach (var upgradeMod in ModList)
        {
            description = upgradeMod.GetUpgradePreviewString(SourceSkill, SourceSkill?.Actor, description);
        }
        return description;
    }

    public override void ApplyUpgrade()
    {
        foreach (var upgradeMod in ModList)
        {
            if (upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.SkillStat)
            {
                Stat stat = SourceSkill.Stats.GetSkillStat(upgradeMod.SkillStatName);
                var mod = upgradeMod.GetModifier();
                mod.Source = SourceSkill;
                mod.IsStackable = true;
                stat.AddModifier(mod);
            }
            else if(upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.ActorStat)
            {
                Stat stat = SourceSkill.Actor.Stats.GetStat(upgradeMod.ActorStatName);
                var mod = upgradeMod.GetModifier();
                mod.Source = SourceSkill;
                mod.IsStackable = true;
                stat.AddModifier(mod);

                if (upgradeMod.ActorStatName == ActorStats.ActorStatTypes.MaxHealth)
                {
                    SourceSkill.Actor.Stats.Health += (int)mod.Value;
                }
            }
            else if(upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.GlobalSkillStat)
            {
                Debug.Log("Global Skill Stat Upgrade");
                foreach(var skill in SourceSkill.Actor.SkillSystem.SkillList)
                {
                    if (!upgradeMod.IsSkillValid(skill))
                    {
                        Debug.Log(skill.Name + " is not valid");
                        continue;
                    }

                    var stat = skill.Stats.GetSkillStat(upgradeMod.SkillStatName);
                    var mod = upgradeMod.GetModifier();
                    mod.Source = SourceSkill;
                    mod.IsStackable = true;
                    stat.AddModifier(mod);

                    Debug.Log("Doing global stat mod for " + skill.Name);
                    Debug.Log(upgradeMod.SkillStatName.ToString());
                    Debug.Log(mod.Value);
                }
            }
        }

        // Stat Changes
        SourceSkill.Actor.Stats.AddArchetypeStats(SourceSkill.Stats.Str, SourceSkill.Stats.Dex, SourceSkill.Stats.Int);
        SourceSkill.Level++;
        SourceSkill.Actor.Tags.AddTags(SourceSkill.Tags);
        SourceSkill.UpgradeHistory.Add(this);

        SourceSkill.Actor.OnUpgrade?.Invoke();
    }

    internal bool IsValid(Skill skill)
    {
        foreach (var skillUpgradeMod in ModList)
        {
            if (skillUpgradeMod.TargetType == UpgradeMod.UpgradeTargetType.SkillStat)
            {
                Stat stat = SourceSkill.Stats.GetSkillStat(skillUpgradeMod.SkillStatName);
                var statName = skillUpgradeMod.SkillStatName;
                var statMod = skillUpgradeMod.GetModifier();
                float previewStatValue = statMod.CalculatePreviewValue(stat, this);

                if (previewStatValue <= 0f)
                {
                    if (statName != SkillStats.SkillStatTypes.Lunge && statName != SkillStats.SkillStatTypes.Knockback)
                    {
                        return false;
                    }
                }

                if (statName == SkillStats.SkillStatTypes.Damage && previewStatValue < 1f) return false;
                if (statName == SkillStats.SkillStatTypes.Count && previewStatValue < 1f) return false;
                if (statName == SkillStats.SkillStatTypes.Rate)
                {
                    if(skill.Stats.Damage.Value > 0)
                    {
                        // 8s, 0.5s
                        if (previewStatValue < 0.125f || previewStatValue > 2f) return false;
                    }
                    else
                    {
                        // 30s, 0.5s
                        if (previewStatValue < 0.033f || previewStatValue > 2f) return false;
                    }
                }
                if (statName == SkillStats.SkillStatTypes.Size && previewStatValue < 0.75f) return false;
                if (statName == SkillStats.SkillStatTypes.Duration && previewStatValue < 0.15f) return false;
                if (statName == SkillStats.SkillStatTypes.Lunge && previewStatValue < -4f) return false;
            }
            else if (skillUpgradeMod.TargetType == UpgradeMod.UpgradeTargetType.ActorStat)
            {
                Stat stat = SourceSkill.Actor.Stats.GetStat(skillUpgradeMod.ActorStatName);
                var statName = skillUpgradeMod.ActorStatName;
                var statMod = skillUpgradeMod.GetModifier();
                float previewStatValue = statMod.CalculatePreviewValue(stat, this);
            }
        }

        return true;
    }
}
