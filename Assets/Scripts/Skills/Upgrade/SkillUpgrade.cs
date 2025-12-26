using Kryz.Stats;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillUpgrade : Upgrade
{
    public List<SkillUpgradeMod> ModList = new();

    public override void ApplyUpgrade()
    {
        throw new System.NotImplementedException();
    }
}
