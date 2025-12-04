using System.Collections.Generic;
using UnityEngine;

public class CombatEncounterObject : MonoBehaviour
{
    public bool IsEncounterOver { get; private set; }
    [SerializeField] GameObject _testEnemyPrefab;
    List<Actor> _enemyList = new();

    private void Start()
    {
        Vector2 playerPos = PlayerManager.I.PlayerList[0].Actor.transform.position;
        Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
        var actor = Instantiate(_testEnemyPrefab, spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform).GetComponent<Actor>();
        _enemyList.Add(actor);
        Debug.Log("Enemy Spawn");
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
