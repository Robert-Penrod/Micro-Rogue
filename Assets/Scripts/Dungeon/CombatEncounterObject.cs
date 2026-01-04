using System.Collections.Generic;
using UnityEngine;

public class CombatEncounterObject : MonoBehaviour
{
    public bool IsEncounterOver { get; private set; }
    [SerializeField] WeightedList<GameObject> _enemyTable = new();
    List<Actor> _enemyList = new();

    public void SpawnEncounter()
    {
        // Init
        float budget = PlayerManager.I.PlayerList.Count * (DungeonManager.I.Data.RoomNumber).ClampMin(0);
        Vector2 playerPos = PlayerManager.I.PlayerList[0].Actor.transform.position;

        // ENEMIES
        //
        // Select enemy types
        var typeTable = new WeightedList<GameObject>();
        var enemyTable = _enemyTable.Clone();
        int typeCount = Random.Range(1, 4);
        for(int i = 0; i < typeCount && enemyTable.Entries.Count > 0; i++)
        {
            typeTable.Add(enemyTable.SelectAndRemoveItem());
        }
        //
        // Spawn Enemies
        int max = (int)budget;
        max = Mathf.Min((int)budget, (int)DungeonManager.I.Data.RoomNumber.Remap(0f, 10f, 3f, 5f, false) * PlayerManager.I.PlayerList.Count);
        int enemyCount = Random.Range(1, 1 + max);
        if (enemyCount == 1 && Random.value > 0.5) enemyCount++;
        for(int i = 0; i < enemyCount && budget >= 1f; i++)
        {
            // Spawn Actor
            budget -= 1f;
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
            var actor = Instantiate(typeTable.SelectItem(), spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
            _enemyList.Add(actor);

            // Base Upgrade
            var upgrades = UpgradeManager.I.GetUpgradeOptions(actor);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();
        }
        //
        // Upgrade Enemies
        while (budget >= 1f)
        {
            var enemyToUpgrade = _enemyList.GetRandomElement();

            // Upgrade
            var upgrades = UpgradeManager.I.GetUpgradeOptions(enemyToUpgrade);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();

            // Hp
            enemyToUpgrade.Stats.HealthMax.BaseValue += Constants.ActorStats.HealthGain;
            enemyToUpgrade.Stats.SetHealthPercent(1f);

            budget -= 1f;
        }
    }

    private void Start()
    {
        SpawnEncounter();

        // Init
        //float budget = 1 + PlayerManager.I.PlayerList.Count + (DungeonManager.I.Data.RoomNumber-1).ClampMin(0);
        //Vector2 playerPos = PlayerManager.I.PlayerList[0].Actor.transform.position;

        // Spawn Enemies
        /*
        int maxEnemies = (int)DungeonManager.I.Data.RoomNumber.Remap(1f, 10f, 2f, 4f, false);
        maxEnemies = maxEnemies.ClampMax((int)(budget + 0.5f));
        int enemyCount = Random.Range(1, maxEnemies + 1);
        for(int i = 0; i < enemyCount && budget >= 1f; i++)
        {
            // Spawn Actor
            budget -= 1.5f;
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
            var actor = Instantiate(_enemyTable.SelectItem(), spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
            _enemyList.Add(actor);

            // Health
            float healthGain = (DungeonManager.I.Data.RoomNumber - 1) * Constants.ActorStats.HealthGain;
            actor.Stats.HealthMax.AddModifier(new Kryz.Stats.StatModifier(healthGain, Kryz.Stats.StatModType.Flat));
            actor.Stats.SetHealthPercent(1f);

            // Base Upgrade
            var upgrades = UpgradeManager.I.GetUpgradeOptions(actor);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();
        }

        // Upgrade Enemies
        while(budget >= 1f)
        {
            var enemyToUpgrade = _enemyList.GetRandomElement();
            var upgrades = UpgradeManager.I.GetUpgradeOptions(enemyToUpgrade);
            if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();
            budget -= 1f;
        }
        */

        /*
        while(budget >= 1f)
        {
            // Spawn Actor
            budget -= 1f;
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
            var actor = Instantiate(_enemyTable.SelectItem(), spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
            _enemyList.Add(actor);

            // Upgrade Actor
            int upgradeCount = 1 + (int)Random.Range(0, budget + 1);
            for(int i = 0; i < upgradeCount; i++)
            {
                var upgrades = UpgradeManager.I.GetUpgradeOptions(actor);
                if (upgrades.Count > 0) upgrades[0].ApplyUpgrade();

                if(i > 0)
                    budget--;
            }

            // Health
            float healthGain = 0.5f * (DungeonManager.I.Data.RoomNumber - 1) * Constants.ActorStats.HealthGain;
            actor.Stats.HealthMax.AddModifier(new Kryz.Stats.StatModifier(healthGain, Kryz.Stats.StatModType.Flat));
            actor.Stats.SetHealthPercent(1f);
        }
        */


        /*
        int enemyCount = PlayerManager.I.PlayerList.Count + (DungeonManager.I.Data.RoomNumber).ClampMin(0) / 4;
        for(int i = 0; i < enemyCount; i++)
        {
            // Spawn Actor
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
            var actor = Instantiate(_enemyTable.SelectItem(), spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
            _enemyList.Add(actor);

            // Upgrade Actor
            var upgrades = UpgradeManager.I.GetUpgradeOptions(actor);
            if(upgrades.Count > 0) upgrades[0].ApplyUpgrade();

            // Health
            float healthGain = 0.5f * (DungeonManager.I.Data.RoomNumber - 1) * Constants.ActorStats.HealthGain;
            actor.Stats.HealthMax.AddModifier(new Kryz.Stats.StatModifier(healthGain, Kryz.Stats.StatModType.Flat));
            actor.Stats.SetHealthPercent(1f);
        }
        Debug.Log("Enemy Spawn");
        */
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
                player.Actor.Rest();
            });
        }
    }
}
