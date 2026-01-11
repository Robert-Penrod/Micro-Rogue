using System.Collections.Generic;
using UnityEngine;

public class SE_StatMod : SkillEffect
{
    public bool IsConstant = true;
    public List<UpgradeMod> Mods = new();

    private void OnEnable()
    {
        Effect();
    }

    public override void Effect()
    {
        foreach(var upgradeMod in Mods)
        {
            if (upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.ActorStat)
            {
                var actorStats = _skill.Actor.Stats;
                var stat = actorStats.GetStat(upgradeMod.ActorStatName);
                string effectTag = _skill.Name + " - " + upgradeMod.ActorStatName.ToString();
                stat.RemoveAllModifiersWithTag(effectTag);
                var mod = upgradeMod.GetModifier();
                mod.Tags.Add(effectTag);
                mod.Source = this;
                mod.IsStackable = true;
                stat.AddModifier(mod);
            }
            else if (upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.GlobalSkillStat)
            {
                foreach (var skill in _skill.Actor.SkillSystem.SkillList)
                {
                    if (!upgradeMod.IsSkillValid(skill)) continue;

                    var stat = skill.Stats.GetSkillStat(upgradeMod.SkillStatName);
                    var mod = upgradeMod.GetModifier();
                    mod.Source = this;
                    mod.IsStackable = true;
                    stat.AddModifier(mod);

                    Debug.Log("Doing global stat mod ");
                    Debug.Log(upgradeMod.SkillStatName.ToString());
                    Debug.Log(mod.Value);
                }
            }
        }
    }
}
