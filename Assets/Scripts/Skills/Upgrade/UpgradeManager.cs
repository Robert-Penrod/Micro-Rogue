using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeManager : Singleton<UpgradeManager>
{
    [Header("Data")]
    [SerializeField] List<Skill> BaseSkillList = new();

    public List<Skill> GetSkillList() => BaseSkillList;

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
        //Debug.Log("Upgrading Players!");
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

    public List<Upgrade> GetUpgradeOptions(Actor actorToUpgrade, int count = 3, float rarityFlip = 0f, float newSkillMult = 1f)
    {
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
        List<Upgrade> upgradeList = new();

        var weightedUpgradeList = GetWeightedUpgradeList(actorToUpgrade, rarityFlip, newSkillMult);
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

    WeightedList<Upgrade> GetWeightedUpgradeList(Actor actorToUpgrade, float minRarity = 0f, float newSkillMult = 1f)
    {
        float tagAffinityMult = actorToUpgrade.IsPlayer() ? 0.25f : 2f;

        if (actorToUpgrade == null || actorToUpgrade.SkillSystem == null) return new();

        // INIT
        WeightedList<Upgrade> weightedUpgradeList = new();

        var skillSystem = actorToUpgrade.SkillSystem;
        var actorSkillList = skillSystem.SkillList;

        //Debug.Log("Skill Count: " + actorSkillList.Count);
        //Debug.Log("\\/==\\/==\\/");

        // INNATE SKILL UPGRADES
        actorToUpgrade.InnateSkillList.ForEach(newSkill =>
        {
            // FILTERS
            //
            // If Player skill must be unlocked
            var unlockedSkillList = Player.PlayerData.GetUnlockedSkillList();
            if (actorToUpgrade.IsPlayer() && !unlockedSkillList.Contains(newSkill.name) && !unlockedSkillList.Contains("all")) return;
            //
            // If no damage skills -> new skill must do damage
            bool hasDamageSkill = actorSkillList.Find(x => x.Stats.Damage.Value > 0) != null;
            if (!hasDamageSkill && newSkill.Stats.Damage.Value <= 0f) return;
            //
            // Actor cannot already have skill
            if (skillSystem.HasSkill(newSkill)) return;
            //
            // Skill Prereq check
            if (!newSkill.ArePrerequisitesMet(actorToUpgrade)) return;
            //
            // Slotsfull check
            int slotCount = 2;
            // -active
            if (actorToUpgrade.SkillSystem.ActiveSkillList.Count >= slotCount && (newSkill.Slot == Skill.SlotEnum.Main || newSkill.Slot == Skill.SlotEnum.Offhand)) return;
            // -passive
            if (actorToUpgrade.SkillSystem.PassiveSkillList.Count >= slotCount && newSkill.Slot == Skill.SlotEnum.Passive) return;
            // Blacklist check
            if (newSkill.Tags.GetTagList().FindAll(tag => actorToUpgrade.BlacklistedTags.Contains(tag)).Count > 0) return;

            weightedUpgradeList.Add(new NewSkillUpgrade(newSkill, actorToUpgrade), 100f);
        });

        // NEW SKILL UPGRADES
        BaseSkillList.ForEach(newSkill =>
        {
            // FILTERS
            //
            // If Player skill must be unlocked
            var unlockedSkillList = Player.PlayerData.GetUnlockedSkillList();
            if (actorToUpgrade.IsPlayer() && !unlockedSkillList.Contains(newSkill.name) && !unlockedSkillList.Contains("all")) return;
            //
            // If no damage skills -> new skill must do damage
            bool hasDamageSkill = actorSkillList.Find(x => x.Stats.Damage.Value > 0) != null;
            if (!hasDamageSkill && newSkill.Stats.Damage.Value <= 0f) return;
            //
            // Actor cannot already have skill
            if (skillSystem.HasSkill(newSkill)) return;
            //
            // Skill Prereq check
            if (!newSkill.ArePrerequisitesMet(actorToUpgrade)) return;
            //
            // Slotsfull check
            int slotCount = 2;
            // -active
            if (actorToUpgrade.SkillSystem.ActiveSkillList.Count >= slotCount && (newSkill.Slot == Skill.SlotEnum.Main || newSkill.Slot == Skill.SlotEnum.Offhand)) return;
            // -passive
            if (actorToUpgrade.SkillSystem.PassiveSkillList.Count >= slotCount && newSkill.Slot == Skill.SlotEnum.Passive) return;
            // Blacklist check
            if(newSkill.Tags.GetTagList().FindAll(tag => actorToUpgrade.BlacklistedTags.Contains(tag)).Count > 0) return;

            // Weight
            float weightMult = 1f / 3f;// newSkill weight
            weightMult *= actorToUpgrade.NewSkillAffinity;
            float weight = Constants.RarityToWeight(newSkill.Rarity) * weightMult;

            // Rarity Flip
            if (minRarity > 0)
            {
                weight = weight.Remap(0f, 1f, minRarity, 1f);
            }

            weightMult *= newSkillMult;

            // Tag Weight
            weight *= actorToUpgrade.Tags.CalculateWeightMultiplier(newSkill.Tags, tagAffinityMult);
            if(newSkill.Tags.GetTagList().Count == 0)
            {
                weight *= 2f;
            }

            //Debug.Log(newSkill.Name + " : " + weight);

            // Add
            weightedUpgradeList.Add(new NewSkillUpgrade(newSkill, actorToUpgrade), weight);
        });

        // SKILL UPGRADES
        actorSkillList.ForEach(actorSkill =>
        {
            // Skill Upgrade list
            var skillUpgradeList = actorSkill.GetUpgradeListClone();

            // Skill Upgrades Loop
            for(int i = 0; i < skillUpgradeList.Entries.Count; i++)
            {
                // Rarity Flip
                if (minRarity > 0) skillUpgradeList.Entries[i].Weight = skillUpgradeList.Entries[i].Weight.Remap(0f, 1f, minRarity, 1f);

                // Tag Weight
                skillUpgradeList.Entries[i].Weight *= actorToUpgrade.Tags.CalculateWeightMultiplier(skillUpgradeList.Entries[i].Item.SourceSkill.Tags, tagAffinityMult * 0.5f);
            }

            // Add
            weightedUpgradeList.AddRange(skillUpgradeList);
        });

        // RETURN
        return weightedUpgradeList;
    }
}
