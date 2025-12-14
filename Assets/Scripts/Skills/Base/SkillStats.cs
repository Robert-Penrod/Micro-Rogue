using UnityEngine;

[System.Serializable]
public class SkillStats
{
    public enum SlotEnum { Main = 0, Offhand = 1, Passive = 2}
    public SlotEnum Slot;

    [Header("Archetype")]
    public int Str;
    public int Dex;
    public int Int;

    [Header("Stats")]
    public Constants.SkillStats.Damage.Label DamageLabel;
    public float Damage => Constants.SkillStats.Damage.LabelToStat(DamageLabel);
    public float Pierce = 1;
    public float Size => Constants.SkillStats.Size.Default;
    //public float Duration => Constants.SkillStats.Duration.Melee;
    public Constants.SkillStats.Duration.Label DurationLabel;
    public float Duration => Constants.SkillStats.Duration.LabelToStat(DurationLabel);

    public float Cooldown => Constants.SkillStats.Cooldown.Default;
    public Constants.SkillStats.Speed.Label SpeedLabel;
    public float Speed => Constants.SkillStats.Speed.LabelToStat(SpeedLabel);
    public float HitboxDelay => Constants.SkillStats.HitboxDelay.Default;
    public float Knockback => Constants.SkillStats.Knockback.Default;
    public float Lunge => Constants.SkillStats.Lunge.Default;
    public Constants.SkillStats.Homing.Label HomingLabel;
    public float Homing => Constants.SkillStats.Homing.LabelToStat(HomingLabel);
}
