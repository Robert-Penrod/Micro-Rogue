using System.Collections;
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
        StartCoroutine(UpgradeCoroutine());
        IEnumerator UpgradeCoroutine()
        {
            for(int i = 0; i < playerList.Count; i++)
            {
                UpgradeMenu.I.DoUpgradeMenuFor(playerList[i].Actor);
                while (IsUpgrading) yield return null;
            }
            yield return null;
        }
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

        var skillSystem = actorToUpgrade.SkillSystem;
        var actorSkillList = skillSystem.SkillList;

        // New Skill Upgrades
        BaseSkillList.ForEach(newSkill =>
        {
            // FILTERS
            //
            // If starting skill -> must do damage
            if (actorSkillList.Count == 0 && newSkill.Stats.Damage.Value <= 0f) return;
            //
            // Actor cannot already have skill
            if (skillSystem.HasSkill(newSkill)) return;

            weightedUpgradeList.Add(new NewSkillUpgrade(newSkill, actorToUpgrade), Constants.RarityToWeight(newSkill.Rarity) / (3f * 3f));
        });

        // Skill Upgrades
        actorSkillList.ForEach(actorSkill =>
        {
            weightedUpgradeList.AddRange(actorSkill.GetUpgradeList());
        });

        // Return
        return weightedUpgradeList;
    }
}
