using UnityEngine;

[System.Serializable]
public class SkillStats
{
    [Header("Archetype")]
    public int Str;
    public int Dex;
    public int Int;

    [Header("Stats")]
    public float Damage => Constants.SkillStats.Damage.Default;
    public float Pierce => Constants.SkillStats.Pierce.Default;
    public float Size => Constants.SkillStats.Size.Default;
    public float Duration => Constants.SkillStats.Duration.Melee;
    public float Cooldown => Constants.SkillStats.Cooldown.Default;
    public float Speed => Constants.SkillStats.Speed.Default;
    public float HitboxDelay => Constants.SkillStats.HitboxDelay.Default;
    public float Knockback => Constants.SkillStats.Knockback.Default;
    public float Lunge => Constants.SkillStats.Lunge.Default;
}
