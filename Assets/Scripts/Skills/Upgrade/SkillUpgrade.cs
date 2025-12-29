using Kryz.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillUpgrade : Upgrade
{
    public List<SkillUpgradeMod> ModList = new();

    public override string GetSlot() => _sourceSkill.Slot.ToString();

    public override string GetTitle() => $"{_sourceSkill.Name} ← {_name}";

    public override Sprite GetIcon()
    {
        return _sourceSkill.Icon;
    }

    public override Color GetColor()
    {
        if (_sourceSkill.SkillColor != Color.clear) return _sourceSkill.SkillColor;
        return GamePaletteManager.I.Palette.GetSkillColor(_sourceSkill);
    }

    public override string GetDescription()
    {
        string description = string.Empty;// _description + "\n";
        ModList.Sort((x, y) => x.StatName > y.StatName ? 1 : -1);
        foreach (SkillUpgradeMod skillUpgradeMod in ModList)
        {
            Stat stat = _sourceSkill.Stats.GetSkillStat(skillUpgradeMod.StatName);
            string statName = stat.Name;
            var statMod = skillUpgradeMod.GetModifier();
            float previewStatValue = statMod.CalculatePreviewValue(stat, this);
            float value = stat.Value;
            string unit = stat.Unit ?? "";
            float positiveDir = 1f;

            // Invert Rate for Cooldown
            if(skillUpgradeMod.StatName == SkillStats.SkillStatTypes.Rate)
            {
                statName = "Cooldown";
                unit = "s";
                value = 1f / value;
                previewStatValue = 1f / previewStatValue;
                positiveDir = -1f;
            }

            // %
            if(unit.Equals("%"))
            {
                value *= 100f;
                previewStatValue *= 100f;
            }

            string valueChange = Constants.ChangeValueString(value, previewStatValue, positiveDir, unit);
            if (description.Length > 0) description += "\n";
            description += ((statName + ": ").Color("CAD079") + valueChange);
        }
        return description;
    }

    public override void ApplyUpgrade()
    {
        foreach (SkillUpgradeMod upgradeMod in ModList)
        {
            Stat stat = _sourceSkill.Stats.GetSkillStat(upgradeMod.StatName);
            /*
            if (upgradeMod.StatMod.Tags.Count == 0)
            {
                skillStatMod.StatMod.Tags.Add("Upgrade");
                skillStatMod.StatMod.Tags.Add(Name);
            }*/
            stat.AddModifier(upgradeMod.GetModifier());
        }

        // Stat Changes
        _sourceSkill.Actor.Stats.AddArchetypeStats(_sourceSkill.Stats.Str, _sourceSkill.Stats.Dex, _sourceSkill.Stats.Int);
        _sourceSkill.Level++;
        //_skill.ParentActor.AddSkillTags(_skill.Tags);

        _sourceSkill.Actor.OnUpgrade?.Invoke();
    }

    internal bool IsValid()
    {
        foreach (SkillUpgradeMod skillUpgradeMod in ModList)
        {
            Stat stat = _sourceSkill.Stats.GetSkillStat(skillUpgradeMod.StatName);
            var statName = skillUpgradeMod.StatName;
            var statMod = skillUpgradeMod.GetModifier();
            float previewStatValue = statMod.CalculatePreviewValue(stat, this);

            if(previewStatValue <= 0f)
            {
                if(statName != SkillStats.SkillStatTypes.Lunge && statName != SkillStats.SkillStatTypes.Knockback)
                {
                    return false;
                }
            }

            if (statName == SkillStats.SkillStatTypes.Count && previewStatValue < 1f) return false;
            if (statName == SkillStats.SkillStatTypes.Rate && previewStatValue < 0.02f) return false;
            if (statName == SkillStats.SkillStatTypes.Size && previewStatValue < 0.75f) return false;
            if (statName == SkillStats.SkillStatTypes.Duration && previewStatValue < 0.15f) return false;
            if (statName == SkillStats.SkillStatTypes.Lunge && previewStatValue < -4f) return false;
        }
            return true;
    }
}
