using Kryz.Stats;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

[System.Serializable]
public class SkillStats
{
    public enum SkillStatTypes { Damage = 10, Rate = 20, Count = 30, Duration = 40, Speed = 50, Size = 60, Pierce = 70, Knockback = 80, Lunge = 90, Potency = 100}

    [HorizontalGroup("Stats")]
    [VerticalGroup("Stats/Left")] public int Str;
    [VerticalGroup("Stats/Left")] public int Dex;
    [VerticalGroup("Stats/Left")] public int Int;

    [VerticalGroup("Stats/Right")] public Stat Damage = new("Damage");
    [VerticalGroup("Stats/Right")] public Stat Rate = new("Rate", "/s");
    [VerticalGroup("Stats/Right")] public Stat Count = new("Count");
    [VerticalGroup("Stats/Right")] public Stat Duration = new("Duration");
    [VerticalGroup("Stats/Right")] public Stat Speed = new("Speed");
    [VerticalGroup("Stats/Right")] public Stat Size = new("Size", "%");
    [VerticalGroup("Stats/Right")] public Stat Pierce = new("Pierce");
    [VerticalGroup("Stats/Right")] public Stat Knockback = new("Knockback");
    [VerticalGroup("Stats/Right")] public Stat Lunge = new("Lunge");
    [VerticalGroup("Stats/Right")] public Stat Potency = new("Potency", "%");
    public float HitboxDelay => Constants.SkillStats.HitboxDelay;

    internal Stat GetSkillStat(SkillStatTypes statName)
    {
        switch (statName)
        {
            case SkillStatTypes.Damage:
                return Damage;
            case SkillStatTypes.Rate:
                return Rate;
            case SkillStatTypes.Count:
                return Count;
            case SkillStatTypes.Duration:
                return Duration;
            case SkillStatTypes.Speed:
                return Speed;
            case SkillStatTypes.Size:
                return Size;
            case SkillStatTypes.Pierce:
                return Pierce;
            case SkillStatTypes.Knockback:
                return Knockback;
            case SkillStatTypes.Lunge:
                return Lunge;
            case SkillStatTypes.Potency:
                return Potency;
            default:
                return null;
        }
    }
}
