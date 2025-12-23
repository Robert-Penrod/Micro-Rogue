using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : PersistantSingleton<UpgradeManager>
{
    [Header("Data")]
    [SerializeField] List<Skill> BaseSkillList = new();

    // State
    public bool IsUpgrading { get; private set; }

    public void UpgradePlayers()
    {
        var playerList = PlayerManager.I.PlayerList;
        if(playerList.Count == 0)
        {
            Debug.Log("No players to upgrade");
            return;
        }


        IsUpgrading = true;
        Debug.Log("Upgrading Players!");
        UpgradeMenu.I.DoUpgradeMenuFor(playerList[0].Actor);        
    }

    public void FinishUpgrading()
    {
        IsUpgrading = false;
        UpgradeMenu.I.SetMenuOpen(false);
    }

    public List<Upgrade> GetUpgradeOptions(Actor actorToUpgrade, int count = 3)
    {
        List<Upgrade> upgradeList = new();

        var weightedUpgradeList = GetWeightedUpgradeList(actorToUpgrade);
        Debug.Log("Weighted Entry Count: " + weightedUpgradeList.Entries.Count);
        for(int i = 0; i < count && weightedUpgradeList.Entries.Count > 0; i++)
        {
            var selectedItem = weightedUpgradeList.SelectAndRemoveItem();
            upgradeList.Add(selectedItem);
        }

        return upgradeList;
    }

    WeightedList<Upgrade> GetWeightedUpgradeList(Actor actorToUpgrade)
    {
        // Init
        WeightedList<Upgrade> weightedUpgradeList = new();

        // New Skill Upgrades
        BaseSkillList.ForEach(x =>
        {
            Debug.Log("Adding Base Skill " + x.gameObject.name);
            weightedUpgradeList.Add(new NewSkillUpgrade(x, actorToUpgrade), 1f);
        });

        // Return
        return weightedUpgradeList;
    }
}
