using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradeHandler : MonoBehaviour
{
    public List<SkillUpgrade> UpgradeList = new();

    public List<Upgrade> GetUpgrades()
    {
        return new List<Upgrade>(UpgradeList);
    }
}
