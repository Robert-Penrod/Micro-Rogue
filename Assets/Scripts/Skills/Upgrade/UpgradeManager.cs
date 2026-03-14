using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeManager : PersistantSingleton<UpgradeManager>
{
    [Header("Data")]
    [SerializeField] List<Skill> BaseSkillList = new();

    // State
    public bool IsUpgrading { get; private set; }

    public IEnumerator UpgradePlayers_Co(int upgradeLevels = 1)
    {
        var playerList = PlayerManager.I.PlayerList;
        if(playerList.Count == 0)
        {
            Debug.Log("No players to upgrade");
            yield break;
        }


        IsUpgrading = true;
        Debug.Log("Upgrading Players!");
        for (int k = 0; k < upgradeLevels; k++)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                UpgradeMenu.I.DoUpgradeMenuFor(playerList[i].Actor);
                while (UpgradeMenu.I.IsOpen) yield return null;
            }
        }
        IsUpgrading = false;
        yield return null;

        PlayerManager.I.SetUIOwner(null);
    }

    public void FinishUpgrading()
    {
        UpgradeMenu.I.SetMenuOpen(false);
    }

    public List<Upgrade> GetUpgradeOptions(Actor actorToUpgrade, int count = 3, float rarityFlip = 0f)
    {
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
        List<Upgrade> upgradeList = new();

        var weightedUpgradeList = GetWeightedUpgradeList(actorToUpgrade, rarityFlip);
        //Debug.Log("Full List Options: " + weightedUpgradeList.Entries.Count);
        for (int i = 0; i < count && weightedUpgradeList.Entries.Count > 0; i++)
        {
            var selectedItem = weightedUpgradeList.SelectAndRemoveItem();
            upgradeList.Add(selectedItem);
        }
        //Debug.Log("Upgrade options: " + upgradeList.Count);
        upgradeList.ForEach(upgrade =>
        {
            //Debug.Log(upgrade.GetTitle());
        });
        //Debug.Log("Rand Num: " + Random.Range(1, 4));
        return upgradeList;
    }

    WeightedList<Upgrade> GetWeightedUpgradeList(Actor actorToUpgrade, float rarityFlip = 0f)
    {
        // Init
        WeightedList<Upgrade> weightedUpgradeList = new();

        var skillSystem = actorToUpgrade.SkillSystem;
        var actorSkillList = skillSystem.SkillList;

        //Debug.Log("Skill Count: " + actorSkillList.Count);
        Debug.Log("\\/==\\/==\\/");
        // New Skill Upgrades
        BaseSkillList.ForEach(newSkill =>
        {
            // FILTERS
            //
            // If no damage skills -> new skill must do damage
            if (actorSkillList.FindAll(x => x.Stats.Damage.Value > 0).Count == 0 && newSkill.Stats.Damage.Value <= 0f) return;
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
            float newSkillMult = (1f / (1f * 3f * 1f)); // newSkill weight
            newSkillMult *= actorToUpgrade.NewSkillAffinity;
            float weight = Constants.RarityToWeight(newSkill.Rarity) * newSkillMult;

            // Rarity Flip
            if (rarityFlip > 0)
            {
                weight = weight.Remap(0f, 1f, rarityFlip, 1f);
            }

            // Tag Weight
            weight *= actorToUpgrade.Tags.CalculateWeightMultiplier(newSkill.Tags);

            Debug.Log(newSkill.Name + " : " + weight);

            // Add
            weightedUpgradeList.Add(new NewSkillUpgrade(newSkill, actorToUpgrade), weight);
        });

        // Skill Upgrades
        actorSkillList.ForEach(actorSkill =>
        {
            // Skill Upgrade list
            var skillUpgradeList = actorSkill.GetUpgradeListClone();

            // Skill Upgrades Loop
            for(int i = 0; i < skillUpgradeList.Entries.Count; i++)
            {
                // Rarity Flip
                if (rarityFlip > 0) skillUpgradeList.Entries[i].Weight = skillUpgradeList.Entries[i].Weight.Remap(0f, 1f, rarityFlip, 1f);

                // Tag Weight
                skillUpgradeList.Entries[i].Weight *= actorToUpgrade.Tags.CalculateWeightMultiplier(skillUpgradeList.Entries[i].Item.SourceSkill.Tags);
            }

            // Add
            weightedUpgradeList.AddRange(skillUpgradeList);
        });

        // Return
        return weightedUpgradeList;
    }
}
