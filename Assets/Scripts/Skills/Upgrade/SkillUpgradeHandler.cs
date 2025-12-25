using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradeHandler : MonoBehaviour
{
    public List<SkillStatUpgrade> UpgradeList = new();

    public List<Upgrade> GetUpgrades()
    {
        return new List<Upgrade>(UpgradeList);
    }
}
