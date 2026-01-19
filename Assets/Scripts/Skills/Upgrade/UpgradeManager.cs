using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : PersistantSingleton<UpgradeManager>
{
    [Header("Data")]
    [SerializeField] List<Skill> BaseSkillList = new();

    // State
    public bool IsUpgrading { get; private set; }

    public IEnumerator UpgradePlayers_Co()
    {
        var playerList = PlayerManager.I.PlayerList;
        if(playerList.Count == 0)
        {
            Debug.Log("No players to upgrade");
            yield break;
        }


        IsUpgrading = true;
        Debug.Log("Upgrading Players!");
        for (int i = 0; i < playerList.Count; i++)
        {
            UpgradeMenu.I.DoUpgradeMenuFor(playerList[i].Actor);
            while (UpgradeMenu.I.IsOpen) yield return null;
        }
        IsUpgrading = false;
        yield return null;

        PlayerManager.I.SetUIOwner(null);
    }

    public void FinishUpgrading()
    {
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

        Debug.Log("Skill Count: " + actorSkillList.Count);

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
            //
            // Slotsfull check
            int slotCount = 3;
            // -active
            if (actorToUpgrade.SkillSystem.ActiveSkillList.Count >= slotCount && (newSkill.Slot == Skill.SlotEnum.Main || newSkill.Slot == Skill.SlotEnum.Offhand)) return;
            // -passive
            if (actorToUpgrade.SkillSystem.PassiveSkillList.Count >= slotCount && newSkill.Slot == Skill.SlotEnum.Passive) return;

            // Weight
            float newSkillMult = 1f;// 1f / (1f * 3f); // newSkill weight
            newSkillMult *= actorToUpgrade.Tags.CalculateWeightMultiplier(newSkill.Tags); // Tag Weight
            newSkillMult *= actorToUpgrade.NewSkillAffinity;

            // Add
            weightedUpgradeList.Add(new NewSkillUpgrade(newSkill, actorToUpgrade), Constants.RarityToWeight(newSkill.Rarity) * newSkillMult);
        });

        // Skill Upgrades
        actorSkillList.ForEach(actorSkill =>
        {
            // Skill Upgrade list
            var skillUpgradeList = actorSkill.GetUpgradeList();

            // Tag Weight
            for(int i = 0; i < skillUpgradeList.Entries.Count; i++) skillUpgradeList.Entries[i].Weight *= actorToUpgrade.Tags.CalculateWeightMultiplier(skillUpgradeList.Entries[i].Item.SourceSkill.Tags);

            // Add
            weightedUpgradeList.AddRange(skillUpgradeList);
        });

        // Return
        return weightedUpgradeList;
    }
}
