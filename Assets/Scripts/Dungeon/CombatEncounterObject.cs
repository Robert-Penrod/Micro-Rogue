using System.Collections.Generic;
using UnityEngine;

public class CombatEncounterObject : MonoBehaviour
{
    public bool IsEncounterOver { get; private set; }
    [SerializeField] WeightedList<Actor> _enemyTable = new();
    [SerializeField] WeightedList<Actor> _bossTable = new();
    List<Actor> _enemyList = new();

    public void SpawnEncounter(float mult = 1f)
    {
        List<Actor> newEnemiesList = new();
        _spawnNum++;
        Random.InitState(DungeonManager.I.Data.GetSeed());

        // Init
        float budget = PlayerManager.I.PlayerList.Count * (DungeonManager.I.Data.RoomNumber).ClampMin(0);

        budget *= DungeonManager.I.Data.RoomNumber.Remap(1f, 15f, 1f, 1.05f); // 1.05f

        //budget += DungeonManager.I.Data.IsElite ? 1.5f : 0f;
        //budget += DungeonManager.I.Data.IsBoss ? 2f : 0f;
        //budget += DungeonManager.I.Data.IsFinalBoss ? 3f : 0f;


        //budget *= DungeonManager.I.Data.IsElite ? 1.2f : 1f;
        //budget *= DungeonManager.I.Data.IsBoss ? 1.4f : 1f;
        //budget *= DungeonManager.I.Data.IsFinalBoss ? 1.4f : 1f;

        budget *= DungeonManager.I.Data.EliteTier > 0 ? (0.1f * (DungeonManager.I.Data.EliteTier - 1f) + 1.1f) : 1f;
        budget *= DungeonManager.I.Data.IsBoss ? 1.2f : 1f;
        budget *= DungeonManager.I.Data.IsFinalBoss ? 1.3f : 1f;
        budget *= DungeonManager.I.Data.RunTier.Remap(1f, 3f, 1f, 1.1f);

        //if (DungeonManager.I.Data.IsBoss) budget += 1;
        //if (DungeonManager.I.Data.IsFinalBoss) budget += 3;
        budget += DungeonManager.I.Data.EliteTier * 0.25f;
        budget += 0.1f * (DungeonManager.I.Data.RunTier - 1f);


        budget *= mult;

        Vector2 playerPos = PlayerManager.I.PlayerList[0].Actor.transform.position;
        Debug.Log("COMBAT ENCOUNTER: " + budget.ToString());

        // ENEMIES
        //
        // Weight enemy Types
        var enemyTable = new WeightedList<Actor>();
        _enemyTable.Entries.ForEach(enemyEntry =>
        {
            float weight = enemyEntry.Weight;

            var biomeMult = DungeonManager.I.Data.Biome switch
            {
                DungeonManager.BiomeEnum.Wilds => enemyEntry.Item.WildsAffinity,
                DungeonManager.BiomeEnum.Underground => enemyEntry.Item.UndergroundAffinity,
                DungeonManager.BiomeEnum.Dungeon => enemyEntry.Item.DungeonAffinity,
                _ => 1f
            };
            float baseMinAffinity = 0.01f; // 0.2f
            biomeMult = biomeMult < 0? biomeMult.Remap(-1f, 0f, 0f, baseMinAffinity) : biomeMult.Remap(0f, 1f, baseMinAffinity, 1f);
            weight *= biomeMult.Clamp01();
            weight *= enemyEntry.Item.RarityMult;

            enemyTable.Add(enemyEntry.Item, weight);
        });
        //
        // Select enemy types

        var typeTable = new WeightedList<Actor>();
        List<Actor> spawnedBossList = new();
        //var enemyTable = _enemyTable.Clone();
        int typeCount = Random.Range(1, 4);
        for(int i = 0; i < typeCount && enemyTable.Entries.Count > 0; i++)
        {
            var selectedEntry = enemyTable.SelectAndRemoveEntry();
            //Debug.Log("Selected Entry " + selectedEntry.Item.gameObject.name + ", " + selectedEntry.Weight);
            typeTable.Add(selectedEntry.Item, selectedEntry.Weight.Remap(0f, 1f, 0.5f, 1f, false));
        }
        //
        // Spawn Enemies
        int max = (int)(2 + (budget / 2f)).ClampMin(1);
        //int absoluteMax = (int)DungeonManager.I.Data.RoomNumber.Remap(0f, 10f, 3f, 4f, false) * PlayerManager.I.PlayerList.Count;
        //if (DungeonManager.I.Data.IsElite) absoluteMax = (int)(absoluteMax * 1.5f);
        //if (DungeonManager.I.Data.IsBoss) absoluteMax = (absoluteMax / 2).ClampMin(1);
        //max = Mathf.Min((int)budget, absoluteMax);
        var data = DungeonManager.I.Data;
        int enemyCount = Random.Range(1, 1 + max);
        if (enemyCount == 1) enemyCount = Random.Range(1, 1 + max);
        if (data.IsFinalBoss) enemyCount = (int)(0.75f * enemyCount);
        if (data.IsBoss) enemyCount = (int)(0.75f * enemyCount);
        enemyCount = enemyCount.ClampMin(1);
        for(int i = 0; i < enemyCount && budget >= 1f + 0.25f * i; i++)
        {
            var selectedTypeActor = typeTable.SelectItem();
            if (DungeonManager.I.Data.IsFinalBoss && i == 0)
            {
                // Final Boss
                selectedTypeActor = _bossTable.SelectItem();
            }
            else
            {
                // Default Selection
                for (int k = 0; k < 50; k++)
                {
                    if (selectedTypeActor.Difficulty <= budget) break;
                    selectedTypeActor = typeTable.SelectItem();
                }
            }

            // Spawn Actor
            float cost = selectedTypeActor.Difficulty + 0.1f * (i).ClampMin(0); // 0.25f
            budget -= cost;
            Debug.Log("Spawning " + selectedTypeActor.gameObject.name + " for " + cost.ToString());
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
            var actor = Instantiate(selectedTypeActor, spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
            newEnemiesList.Add(actor);

            // Selected boss
            if (DungeonManager.I.Data.IsFinalBoss && i == 0)
            {
                spawnedBossList.Add(actor);
            }

            // Brain Difficulty
            var brain = actor.GetComponent<SimpleNPCBrain>();
            if(brain != null)
            {
                brain.DifficultyMult = 0.9f;
            }

            // Base Upgrade
            var upgrades = UpgradeManager.I.GetUpgradeOptions(actor, tagAffinityMult: 1f);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();
        }
        //
        // Select Boss
        WeightedList<Actor> _upgradeAffinityList = new();
        bool selectedBoss = _spawnNum > 1;
        newEnemiesList.ForEach(enemy =>
        {
            bool isBoss = spawnedBossList.Contains(enemy) || (DungeonManager.I.Data.IsBoss && !selectedBoss);
            float affinityMult = isBoss ? 7f : 1f;
            if (isBoss && !selectedBoss)
            {
                selectedBoss = true;
                enemy._initScale *= 1.25f;
                enemy.Stats.HealthMax.BaseValue *= 1.05f;

                // Brain Update
                var brain = enemy.GetComponent<SimpleNPCBrain>();
                if (brain != null)
                {
                    brain.DifficultyMult = 1.2f;
                }
            }
            _upgradeAffinityList.Add(enemy, affinityMult * enemy.UpgradeAffinity);
        });
        //
        // Upgrade Enemies
        while (budget >= 1f)
        {
            var enemyToUpgrade = _upgradeAffinityList.SelectItem();

            // Upgrade
            float rarityFlip = Random.value < 0.25f ? 5f : 0f;
            float newSkillMult = Random.value < 0.25f ? 5f : 1f;
            var upgrades = UpgradeManager.I.GetUpgradeOptions(enemyToUpgrade, rarityFlip: rarityFlip, newSkillMult: newSkillMult, tagAffinityMult: 1f);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();

            // Hp
            enemyToUpgrade.Stats.HealthMax.BaseValue += Constants.ActorStats.HealthGain;
            enemyToUpgrade.Stats.SetHealthPercent(1f);

            budget -= 1f;
        }

        newEnemiesList.ForEach(enemy =>
        {
            enemy.Stats.SetHealthPercent(1f);
        });

        _enemyList.AddRange(newEnemiesList);
    }

    float _spawnTick;
    float _spawnTime = 30f;
    int _spawnNum = 0;

    private void Start()
    {
        SpawnEncounter();
    }

    private void FixedUpdate()
    {
        // Wave Spawner
        /*
        if (!IsEncounterOver)
        {
            _spawnTick += Time.fixedDeltaTime;
            if (_spawnTick >= (_spawnNum).Remap(1f, 2f, 30f, 20f))
            {
                _spawnTick = 0f;
                var difMult = (_spawnNum).Remap(1f, 2f, 0.25f, 0.75f);
                SpawnEncounter(difMult);
            }
        }
        */

        for(int i = 0; i < _enemyList.Count; i++)
        {
            if(_enemyList[i] == null || !_enemyList[i].IsAlive)
            {
                _enemyList.RemoveAt(i);
                i--;
            }
        }

        if(_enemyList.Count == 0 && !IsEncounterOver)
        {
            IsEncounterOver = true;
            DungeonManager.I.SpawnPortals();
            PlayerManager.I.PlayerList.ForEach(player =>
            {
                this.DelayedInvoke(0.5f, () =>
                {
                    player.Actor.Rest();
                });
            });
        }
    }
}
