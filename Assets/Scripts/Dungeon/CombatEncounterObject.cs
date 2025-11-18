using UnityEngine;

public class CombatEncounterObject : MonoBehaviour
{
    [SerializeField] GameObject _testEnemyPrefab;

    private void Start()
    {
        Vector2 playerPos = PlayerManager.I.PlayerList[0].Actor.transform.position;
        Vector2 spawnPos = SpawnSystem.GetRandomEmptyPosAvoidingCircle(Vector2.zero, 1f, playerPos, 5f);
        Instantiate(_testEnemyPrefab, spawnPos, Quaternion.identity, DungeonManager.I.DungeonTransform);
        Debug.Log("Enemy Spawn");
    }
}
