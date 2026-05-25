using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField] GameObject _wildsRoomShapes;
    [SerializeField] GameObject _undergroundRoomShapes;
    [SerializeField] GameObject _dungeonRoomShapes;

    [Header("Tables")]
    [SerializeField] WeightedList<GameObject> _wildsProps = new();
    [SerializeField] WeightedList<GameObject> _undergroundProps = new();
    [SerializeField] WeightedList<GameObject> _dungeonProps = new();

    DungeonManager _dungeonManager;

    private void Start()
    {
        _dungeonManager = DungeonManager.I;
        Generate();
    }

    void Generate()
    {
        // Random Init
        Random.InitState(DungeonManager.I.Data.GetSeed());

        // Room Shape
        var biome = _dungeonManager.Data.Biome;
        _wildsRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Wilds);
        _undergroundRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Underground);
        _dungeonRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Dungeon);

        // Scale
        transform.localScale = Random.Range(0.9f, 1.1f) * DungeonManager.I.Data.Coordinate.y.Remap(1f, 15f, 1f, 1.125f) * Vector3.one;

        // Props
        WeightedList<GameObject> biomePropTable = biome switch
        {
            DungeonManager.BiomeEnum.Wilds => _wildsProps,
            DungeonManager.BiomeEnum.Underground => _undergroundProps,
            DungeonManager.BiomeEnum.Dungeon => _dungeonProps,
            _ => null
        };
        if (biomePropTable != null && biomePropTable.Entries.Count > 0)
        {
            float propCount = Random.Range(1, 5) + Random.Range(1, 5); // 2d4
            propCount *= DungeonManager.I.Data.RunTier.Remap(1f, 3f, 1f, 1.25f);
            propCount *= ((int)DungeonManager.I.Data.Biome).Remap(1f, 3f, 1f, 1.25f);
            for (int i = 0; i < propCount; i++)
            {
                var propPrefab = biomePropTable.SelectItem();
                Instantiate(propPrefab, SpawnSystem.GetRandomEmptyPos(), Quaternion.identity, this.transform);
            }
        }
    }
}
