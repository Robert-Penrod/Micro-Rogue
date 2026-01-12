using Kryz.Stats;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UpgradeMod
{
    public enum UpgradeTargetType { SkillStat = 0, GlobalSkillStat = 10, ActorStat = 20 }

    [BoxGroup("Filter")]
    public UpgradeTargetType TargetType;

    // Actor
    [BoxGroup("Filter"), ShowIf(nameof(IsActor))]
    public ActorStats.ActorStatTypes ActorStatName;

    // Skill
    bool _showSkillStatName => IsSkill || IsGlobalSkill;
    [BoxGroup("Filter"), ShowIf(nameof(_showSkillStatName))]
    public SkillStats.SkillStatTypes SkillStatName;
    [BoxGroup("Filter"), ShowIf(nameof(IsGlobalSkill))]
    public List<Skill.SlotEnum> Slots = new();
    [BoxGroup("Filter"), ShowIf(nameof(IsGlobalSkill))]
    public bool IsStr;
    [BoxGroup("Filter"), ShowIf(nameof(IsGlobalSkill))]
    public bool IsDex;
    [BoxGroup("Filter"), ShowIf(nameof(IsGlobalSkill))]
    public bool IsInt;

    [BoxGroup("Mod")]
    public float BalancePoints;

    private bool IsActor => TargetType == UpgradeTargetType.ActorStat;
    private bool IsSkill => TargetType == UpgradeTargetType.SkillStat;
    private bool IsGlobalSkill => TargetType == UpgradeTargetType.GlobalSkillStat;

    public bool IsSkillValid(Skill skill)
    {
        if (Slots.Count > 0 && !Slots.Contains(skill.Slot)) return false;
        if (IsStr && skill.Stats.Str == 0) return false;
        if (IsDex && skill.Stats.Dex == 0) return false;
        if (IsInt && skill.Stats.Int == 0) return false;
        return true;
    }

    public string GetUpgradePreviewString(Skill sourceSkill, Actor upgradeActor, string description)
    {
        if(TargetType == UpgradeTargetType.SkillStat)
        {
            return GetSkillUpgradeDescription(sourceSkill, description, this);
        }
        else if(TargetType == UpgradeTargetType.ActorStat)
        {
            return GetActorUpgradeDescription(upgradeActor, description, this);
        }
        else if(TargetType == UpgradeTargetType.GlobalSkillStat)
        {
            return GetGlobalSkillUpgradeDescription(sourceSkill, description, this);
        }
        return string.Empty;
    }

    public string GetGlobalSkillUpgradeDescription(Skill sourceSkill, string description, UpgradeMod skillUpgradeMod)
    {
        string statName = string.Empty;
        // Slot
        foreach(var slot in Slots)
        {
            statName += slot.ToString() + " ";
        }
        // Archetype
        if (IsStr) statName += "Str ";
        if (IsDex) statName += "Dex ";
        if (IsInt) statName += "Int ";
        //
        statName += skillUpgradeMod.SkillStatName.ToString();

        float positiveDir = 1f;
        string unit = "%";

        float value = 0f;// sourceSkill.UpgradeHistory.FindAll(x => x.GetTitle() == skillUpgradeMod.;
        float previewStatValue = skillUpgradeMod.GetModifier().Value;

        // %
        if (unit.Equals("%"))
        {
            value *= 100f;
            previewStatValue *= 100f;
        }

        string valueChange = Constants.ChangeValueString(value, previewStatValue, positiveDir, unit, true);
        if (description.Length > 0) description += "\n";
        description += ((statName + ": ").Color("CAD079") + valueChange);

        return description;
    }

    public string GetActorUpgradeDescription(Actor actor, String description, UpgradeMod actorUpgradeMod)
    {
        Stat stat = actor.Stats.GetStat(actorUpgradeMod.ActorStatName);
        string statName = stat.Name;
        var statMod = actorUpgradeMod.GetModifier();
        float previewStatValue = statMod.CalculatePreviewValue(stat, this);
        float value = stat.Value;
        string unit = stat.Unit ?? "";
        float positiveDir = 1f;

        // %
        if (unit.Equals("%"))
        {
            value *= 100f;
            previewStatValue *= 100f;
        }

        string valueChange = Constants.ChangeValueString(value, previewStatValue, positiveDir, unit);
        if (description.Length > 0) description += "\n";
        description += ((statName + ": ").Color("CAD079") + valueChange);
        return description;
    }

    public string GetSkillUpgradeDescription(Skill sourceSkill, string description, UpgradeMod skillUpgradeMod)
    {
        Stat stat = sourceSkill.Stats.GetSkillStat(skillUpgradeMod.SkillStatName);
        string statName = stat.Name;
        var statMod = skillUpgradeMod.GetModifier();
        float previewStatValue = statMod.CalculatePreviewValue(stat, this);
        float value = stat.Value;
        string unit = stat.Unit ?? "";
        float positiveDir = 1f;

        // Invert Rate for Cooldown
        if (skillUpgradeMod.SkillStatName == SkillStats.SkillStatTypes.Rate)
        {
            statName = "Cooldown";
            unit = "s";
            value = 1f / value;
            previewStatValue = 1f / previewStatValue;
            positiveDir = -1f;
        }

        // %
        if (unit.Equals("%"))
        {
            value *= 100f;
            previewStatValue *= 100f;
        }

        string valueChange = Constants.ChangeValueString(value, previewStatValue, positiveDir, unit);
        if (description.Length > 0) description += "\n";
        description += ((statName + ": ").Color("CAD079") + valueChange);
        return description;
    }

    public StatModifier GetModifier()
    {
        return TargetType switch
        {
            UpgradeTargetType.ActorStat => BalancePoints.BPToStatMod(ActorStatName),
            UpgradeTargetType.SkillStat => BalancePoints.BPToStatMod(SkillStatName),
            UpgradeTargetType.GlobalSkillStat => BalancePoints.BPToStatMod(SkillStatName),
            _ => null
        };
    }
}
