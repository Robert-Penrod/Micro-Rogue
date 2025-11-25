using UnityEngine;

[System.Serializable]
public class SkillStats
{
    public float Size = Constants.SkillStats.Size.Default;
    public float Duration = Constants.SkillStats.Duration.Melee;
    public float Cooldown = Constants.SkillStats.Cooldown.Default;
    public float Speed = Constants.SkillStats.Speed.Default;
}
