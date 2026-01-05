using Kryz.Stats;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UpgradeMod : ISerializationCallbackReceiver
{
    public enum UpgradeTargetType { ActorStat, SkillStat }

    [BoxGroup("Filter")]
    public UpgradeTargetType TargetType;

    // Actor
    [BoxGroup("Filter"), ShowIf(nameof(IsActor))]
    public ActorStats.ActorStatTypes ActorStatName;

    // Skill
    [BoxGroup("Filter"), ShowIf(nameof(IsSkill))]
    public SkillStats.SkillStatTypes SkillStatName;

    [BoxGroup("Filter"), ShowIf(nameof(IsSkill))]
    public bool IsStr;
    [BoxGroup("Filter"), ShowIf(nameof(IsSkill))]
    public bool IsDex;
    [BoxGroup("Filter"), ShowIf(nameof(IsSkill))]
    public bool IsInt;

    [BoxGroup("Mod")]
    public float BalancePoints;

    // --------------------
    // Legacy (from SkillUpgradeMod)
    // --------------------
    // This field name matches the OLD class field name exactly.
    // Unity will load old prefab data into it even though the class changed.
    [SerializeField, HideInInspector]
    private SkillStats.SkillStatTypes StatName;

    [SerializeField, HideInInspector]
    private bool _migrated;

    private bool IsActor => TargetType == UpgradeTargetType.ActorStat;
    private bool IsSkill => TargetType == UpgradeTargetType.SkillStat;

    public StatModifier GetModifier()
    {
        return TargetType switch
        {
            UpgradeTargetType.ActorStat => BalancePoints.BPToStatMod(ActorStatName),
            UpgradeTargetType.SkillStat => BalancePoints.BPToStatMod(SkillStatName),
            _ => null
        };
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        // If this object came from old data, StatName will be populated,
        // and _migrated will be false because it didn't exist before.
        if (!_migrated)
        {
            // Migrate old SkillUpgradeMod data => new format
            SkillStatName = StatName;
            TargetType = UpgradeTargetType.SkillStat;

            // sensible defaults; tweak if you want
            IsStr = IsDex = IsInt = false;

            _migrated = true;
        }
    }
}
