using Kryz.Stats;
using UnityEngine;

public static class BalancePoint
{
    public static float BP_PercentDPSGain = 0.5f;

    // Skills
    public static float BP_Skill_Damage = 1f * BP_PercentDPSGain;
    public static float BP_Skill_RandomDamage = 2f * BP_PercentDPSGain;
    public static float BP_Skill_Rate = 1f * BP_PercentDPSGain;
    public static float BP_Skill_Size = 1f * BP_PercentDPSGain;
    public static float BP_Skill_Duration = 1f * BP_PercentDPSGain;
    public static float BP_Skill_Speed = 1f * BP_PercentDPSGain;
    public static float BP_Skill_Count = 1f * BP_PercentDPSGain;
    public static float BP_Skill_Pierce = 2f * BP_PercentDPSGain;
    public static float BP_Skill_Lunge = 2f * BP_PercentDPSGain;

    // Actor
    public static float BP_Actor_MaxHealth = 5f;
    public static float BP_Actor_Speed = 0.25f;
    public static float BP_Actor_Evasion = 1f;
    public static float BP_Actor_Defense = 1f;
    public static float BP_Actor_DodgeRate = 0.1f;

    public static StatModifier BPToStatMod(this float bp, ActorStats.ActorStatTypes actorStatType)
    {
        float value = 0f;
        StatModType modType = StatModType.PercentAdd;

        switch (actorStatType)
        {
            case ActorStats.ActorStatTypes.MaxHealth:
                value = BP_Actor_MaxHealth;
                modType = StatModType.Flat;
                break;
            case ActorStats.ActorStatTypes.Evasion:
                value = BP_Actor_Evasion;
                modType = StatModType.Flat;
                break;
            case ActorStats.ActorStatTypes.Defense:
                value = BP_Actor_Defense;
                modType = StatModType.Flat;
                break;
            case ActorStats.ActorStatTypes.DodgeRate:
                value = BP_Actor_DodgeRate;
                modType = StatModType.Flat;
                break;
            case ActorStats.ActorStatTypes.MoveSpeed:
                value = BP_Actor_Speed;
                modType = StatModType.Flat;
                break;
        }

        return new StatModifier(
            bp * value,
            modType
        );
    }

    public static StatModifier BPToStatMod(this float bp, SkillStats.SkillStatTypes skillStatType)
    {
        float multiplier = skillStatType switch
        {
            SkillStats.SkillStatTypes.Damage => BP_Skill_Damage,
            SkillStats.SkillStatTypes.RandomDamage => BP_Skill_RandomDamage,
            SkillStats.SkillStatTypes.Rate => BP_Skill_Rate,
            SkillStats.SkillStatTypes.Count => BP_Skill_Count,
            SkillStats.SkillStatTypes.Size => BP_Skill_Size,
            SkillStats.SkillStatTypes.Duration => BP_Skill_Duration,
            SkillStats.SkillStatTypes.Speed => BP_Skill_Speed,
            SkillStats.SkillStatTypes.Pierce => BP_Skill_Pierce,
            SkillStats.SkillStatTypes.Knockback => BP_Skill_Damage,
            SkillStats.SkillStatTypes.Lunge => BP_Skill_Lunge,
            //SkillStats.SkillStatTypes.Potency => BP_Skill_Damage,

            _ => 1f
        };

        float value = bp * multiplier;

        return new StatModifier(
            value,
            StatModType.PercentAdd
        );
    }
}
