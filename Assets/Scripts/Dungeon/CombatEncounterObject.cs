using System.Collections.Generic;
using UnityEngine;

public class CombatEncounterObject : MonoBehaviour
{
    public bool IsEncounterOver { get; private set; }
    [SerializeField] WeightedList<Actor> _enemyTable = new();
    List<Actor> _enemyList = new();

    public void SpawnEncounter()
    {
        Random.InitState(DungeonManager.I.Data.GetSeed());

        // Init
        float budget = PlayerManager.I.PlayerList.Count * (DungeonManager.I.Data.RoomNumber).ClampMin(0);
        //budget *= DungeonManager.I.Data.IsElite ? 1.125f : 1f;
        //budget *= DungeonManager.I.Data.IsBoss ? 1.25f : 1f;
        budget += DungeonManager.I.Data.IsElite ? 1.5f : 0f;
        budget += DungeonManager.I.Data.IsBoss ? 3f : 0f;
        Vector2 playerPos = PlayerManager.I.PlayerList[0].Actor.transform.position;

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
            biomeMult = biomeMult < 0? 0f : biomeMult.Remap(0f, 1f, 0.2f, 1f);
            weight *= biomeMult;
            weight *= enemyEntry.Item.RarityMult;
            weight = 1f;

            enemyTable.Add(enemyEntry.Item, weight);
        });
        //
        // Select enemy types
        var typeTable = new WeightedList<Actor>();
        //var enemyTable = _enemyTable.Clone();
        int typeCount = Random.Range(1, 4);
        for(int i = 0; i < typeCount && enemyTable.Entries.Count > 0; i++)
        {
            var selectedEntry = enemyTable.SelectAndRemoveEntry();
            typeTable.Add(selectedEntry.Item, selectedEntry.Weight.Remap(0f, 1f, 0.5f, 1f, false));
        }
        //
        // Spawn Enemies
        int max = (int)budget;
        int absoluteMax = (int)DungeonManager.I.Data.RoomNumber.Remap(0f, 10f, 3f, 4f, false) * PlayerManager.I.PlayerList.Count;
        if (DungeonManager.I.Data.IsElite) absoluteMax = (int)(absoluteMax * 1.5f);
        //if (DungeonManager.I.Data.IsBoss) absoluteMax = (absoluteMax / 2).ClampMin(1);
        max = Mathf.Min((int)budget, absoluteMax);
        int enemyCount = Random.Range(1, 1 + max);
        if (enemyCount == 1 && Random.value < 0.75) enemyCount++;
        for(int i = 0; i < enemyCount && budget >= 1f; i++)
        {
            var selectedTypeActor = typeTable.SelectItem();
            for(int k = 0; k < 50; k++)
            {
                if (selectedTypeActor.Difficulty <= budget) break;
                selectedTypeActor = typeTable.SelectItem();
            }

            // Spawn Actor
            budget -= selectedTypeActor.Difficulty;
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
            var actor = Instantiate(selectedTypeActor, spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
            _enemyList.Add(actor);

            // Base Upgrade
            var upgrades = UpgradeManager.I.GetUpgradeOptions(actor);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();
        }
        //
        // Upgrade Enemies
        WeightedList<Actor> _upgradeAffinityList = new();
        bool selectedBoss = false;
        _enemyList.ForEach(enemy =>
        {
            bool isBoss = DungeonManager.I.Data.IsBoss && !selectedBoss;
            float affinityMult = isBoss ? 5f : 1f;
            if (isBoss && !selectedBoss)
            {
                selectedBoss = true;
                enemy._initScale *= 1.25f;
            }
            _upgradeAffinityList.Add(enemy, affinityMult * enemy.UpgradeAffinity);
        });
        while (budget >= 1f)
        {
            var enemyToUpgrade = _upgradeAffinityList.SelectItem();

            // Upgrade
            var upgrades = UpgradeManager.I.GetUpgradeOptions(enemyToUpgrade);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();

            // Hp
            enemyToUpgrade.Stats.HealthMax.BaseValue += Constants.ActorStats.HealthGain;
            enemyToUpgrade.Stats.SetHealthPercent(1f);

            budget -= 1f;
        }

        _enemyList.ForEach(enemy =>
        {
            enemy.Stats.SetHealthPercent(1f);
        });
    }

    private void Start()
    {
        SpawnEncounter();
    }

    private void FixedUpdate()
    {
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
