using Kryz.Stats;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UpgradeMod
{
    #region Filtering
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
    #endregion

    public static void ApplyModList(List<UpgradeMod> modList, SkillInstance skillInstance) => ApplyModList(modList, skillInstance.Skill, skillInstance);
    public static void ApplyModList(List<UpgradeMod> modList, Skill sourceSkill, SkillInstance sourceSkillInstance = null)
    {
        object source = sourceSkillInstance;
        if (source == null) source = sourceSkill;

        foreach (var upgradeMod in modList)
        {
            if (upgradeMod.TargetType == UpgradeTargetType.ActorStat)
            {
                var actorStats = sourceSkill.Actor.Stats;
                var stat = actorStats.GetStat(upgradeMod.ActorStatName);
                string effectTag = sourceSkill.Name + " - " + upgradeMod.ActorStatName.ToString();
                stat.RemoveAllModifiersWithTag(effectTag);
                var mod = upgradeMod.GetModifier();
                mod.Tags.Add(source.ToString());
                mod.Source = source;
                mod.IsStackable = true;
                stat.AddModifier(mod);
            }
            else if (upgradeMod.TargetType == UpgradeTargetType.GlobalSkillStat)
            {
                foreach (var skill in sourceSkill.Actor.SkillSystem.SkillList)
                {
                    Debug.Log($"Appling {sourceSkill.Name} upgrade to {skill.Name}");
                    if (skill == sourceSkill)
                    {
                        continue;
                    }
                    if (!upgradeMod.IsSkillValid(skill)) continue;

                    var stat = skill.Stats.GetSkillStat(upgradeMod.SkillStatName);
                    var mod = upgradeMod.GetModifier();
                    string effectTag = sourceSkill.Name;
                    mod.Tags.Add(source.ToString());
                    mod.Source = source;
                    mod.IsStackable = true;
                    stat.AddModifier(mod);

                    Debug.Log("Doing global stat mod ");
                    Debug.Log(upgradeMod.SkillStatName.ToString());
                    Debug.Log(mod.Value);
                }
            }
        }
    }

    public static void RemoveModList(List<UpgradeMod> modList, SkillInstance sourceSkillInstance) => RemoveModList(modList, sourceSkillInstance.Skill, sourceSkillInstance);
    public static void RemoveModList(List<UpgradeMod> modList, Skill sourceSkill, SkillInstance sourceSkillInstance = null)
    {
        object source = sourceSkillInstance;
        if (source == null) source = sourceSkill;

        foreach (var upgradeMod in modList)
        {
            if (upgradeMod.TargetType == UpgradeTargetType.ActorStat)
            {
                modList.ForEach(mod => {
                    var actorStat = sourceSkillInstance.Skill.Actor.Stats.GetStat(mod.ActorStatName);
                    actorStat.RemoveAllModifiersFromSource(source);
                });
            }
            else if (upgradeMod.TargetType == UpgradeTargetType.GlobalSkillStat)
            {
                foreach (var skill in sourceSkillInstance.Skill.Actor.SkillSystem.SkillList)
                {
                    modList.ForEach(mod => {
                        var skillStat = skill.Stats.GetSkillStat(mod.SkillStatName);
                        skillStat.RemoveAllModifiersFromSource(source);
                    });
                }
            }
        }
    }

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
        if (SkillStatName == SkillStats.SkillStatTypes.Pyro && (skill.Stats.Frost.BaseValue > 0 || skill.Stats.Static.BaseValue > 0)) return false;
        if (SkillStatName == SkillStats.SkillStatTypes.Frost && (skill.Stats.Pyro.BaseValue > 0 || skill.Stats.Static.BaseValue > 0)) return false;
        if (SkillStatName == SkillStats.SkillStatTypes.Static && (skill.Stats.Pyro.BaseValue > 0 || skill.Stats.Frost.BaseValue > 0)) return false;
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
            return GetGlobalSkillUpgradeDescription(upgradeActor, sourceSkill, description, this);
        }
        return string.Empty;
    }

    public string GetGlobalSkillUpgradeDescription(Actor actor, Skill sourceSkill, string description, UpgradeMod skillUpgradeMod)
    {
        string statName = string.Empty;
        statName += "Global ";
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

        var statMod = skillUpgradeMod.GetModifier();
        if (actor == null)
        {
            if (description.Length > 0) description += "\n";
            description += (statName + ": ").Color(Constants.Colors.LabelColorHex) + statMod.ToString();
            return description;
        }

        float positiveDir = 1f;
        string unit = "%";

        // Aggregate values from upgrade history
        float value = 0f;
        //sourceSkill.UpgradeHistory.ForEach(skillUpgrade =>
        //{
        actor = UpgradeMenu.I._actorToUpgrade;
        actor.SkillSystem.SkillList.ForEach(skill =>
        {
            skill.UpgradeHistory.ForEach(skillUpgrade =>
            {
                skillUpgrade.ModList.ForEach(upgradeMod =>
                {
                    if (upgradeMod.TargetType == UpgradeTargetType.GlobalSkillStat)
                    {
                        if (upgradeMod.SkillStatName == skillUpgradeMod.SkillStatName)
                        {
                            value += upgradeMod.GetModifier().Value;
                        }
                    }
                });
            });
        });
        float previewStatValue = value + skillUpgradeMod.GetModifier().Value;

        // %
        if (unit.Equals("%"))
        {
            value *= 100f;
            previewStatValue *= 100f;
        }

        string valueChange = Constants.ChangeValueString(value, previewStatValue, positiveDir, unit, true);
        if (description.Length > 0) description += "\n";
        description += ((statName + ": ").Color(Constants.Colors.LabelColorHex) + valueChange);

        return description;
    }

    public string GetActorUpgradeDescription(Actor actor, String description, UpgradeMod actorUpgradeMod)
    {
        if(actor == null) actor = UpgradeMenu.I._actorToUpgrade;

        var statMod = actorUpgradeMod.GetModifier();
        if(actor == null) return (actorUpgradeMod.ActorStatName.ToString() + ": ").Color(Constants.Colors.LabelColorHex) + statMod.ToString();

        Stat stat = actor.Stats.GetStat(actorUpgradeMod.ActorStatName);
        string statName = stat.Name;
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

        // Invert Rate for DodgeCooldown
        if (actorUpgradeMod.ActorStatName == ActorStats.ActorStatTypes.DodgeRate)
        {
            statName = "Dodge Cooldown";
            unit = "s";
            value = 1f / value;
            previewStatValue = 1f / previewStatValue;
            positiveDir = -1f;
        }

        string valueChange = Constants.ChangeValueString(value, previewStatValue, positiveDir, unit);
        if (description.Length > 0) description += "\n";
        description += ((statName + ": ").Color(Constants.Colors.LabelColorHex) + valueChange);
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
        description += ((statName + ": ").Color(Constants.Colors.LabelColorHex) + valueChange);
        return description;
    }

    public StatModifier GetModifier()
    {
        int tagFilterCount = 0;
        if (IsStr) tagFilterCount++;
        if (IsDex) tagFilterCount++;
        if (IsInt) tagFilterCount++;

        return TargetType switch
        {
            UpgradeTargetType.ActorStat => BalancePoints.BPToStatMod(ActorStatName),
            UpgradeTargetType.SkillStat => BalancePoints.BPToStatMod(SkillStatName),
            UpgradeTargetType.GlobalSkillStat => BalancePoints.BPToStatMod(SkillStatName, true, tagFilterCount),
            _ => null
        };
    }
}
