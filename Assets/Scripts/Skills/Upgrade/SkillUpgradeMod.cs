using Kryz.Stats;
using System;
using UnityEngine;

[System.Serializable]
public class SkillUpgradeMod
{
    public SkillStats.SkillStatTypes StatName;
    public float BalancePoints;
    //public StatModifier Modifier;

    public StatModifier GetModifier()
    {
        return BalancePoints.BPToStatMod(StatName);
    }
}
