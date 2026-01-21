using System.Collections.Generic;
using UnityEngine;

public class CombatEncounterObject : MonoBehaviour
{
    public bool IsEncounterOver { get; private set; }
    [SerializeField] WeightedList<GameObject> _enemyTable = new();
    List<Actor> _enemyList = new();

    public void SpawnEncounter()
    {
        Random.InitState(DungeonManager.I.Data.GetSeed());

        // Init
        float budget = PlayerManager.I.PlayerList.Count * (DungeonManager.I.Data.RoomNumber).ClampMin(0);
        budget *= DungeonManager.I.Data.IsElite ? 1.25f : 1f;
        budget *= DungeonManager.I.Data.IsBoss ? 1.25f : 1f;
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
        int absoluteMax = (int)DungeonManager.I.Data.RoomNumber.Remap(0f, 10f, 3f, 4f, false) * PlayerManager.I.PlayerList.Count;
        if (DungeonManager.I.Data.IsElite) absoluteMax = (int)(absoluteMax * 1.5f);
        max = Mathf.Min((int)budget, absoluteMax);
        int enemyCount = Random.Range(1, 1 + max);
        if (enemyCount == 1 && Random.value < 0.75) enemyCount++;
        for(int i = 0; i < enemyCount && budget >= 1f; i++)
        {
            var selectedTypeActor = typeTable.SelectItem().GetComponent<Actor>();
            for(int k = 0; k < 50; k++)
            {
                if (selectedTypeActor.Difficulty <= budget) break;
                selectedTypeActor = typeTable.SelectItem().GetComponent<Actor>();
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
            float affinityMult = DungeonManager.I.Data.IsBoss && !selectedBoss ? 10f : 1f;
            if (!selectedBoss) selectedBoss = true;
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
