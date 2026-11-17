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

    Transform _propHolder;

    DungeonManager _dungeonManager;

    private void Start()
    {
        _dungeonManager = DungeonManager.I;
        Generate();
    }

    private void OnEnable()
    {
        _dungeonManager = DungeonManager.I;
        if (_dungeonManager == null) return;
        Generate();
    }

    void Generate()
    {
        if (_propHolder == null)
        {
            _propHolder = new GameObject("PropParent").transform;
            _propHolder.name = "Props";
            _propHolder.SetParent(transform);
        }
        else
        {
            _propHolder.DestroyChilden();
        }

        // Random Init
        int seed = _dungeonManager?.Data?.GetSeed() ?? System.DateTime.Now.Ticks.GetHashCode();
        Random.InitState(seed);
        Utils.RandomSeed();

        // Room Shape
        var biome = _dungeonManager?.Data.Biome;
        _wildsRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Forest);
        _undergroundRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Cave);
        _dungeonRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Dungeon);

        // Scale
        transform.localScale = Random.Range(0.9f, 1.1f) * _dungeonManager.Data.Coordinate.y.Remap(1f, 15f, 1f, 1.125f) * Vector3.one;

        // Props
        WeightedList<GameObject> biomePropTable = biome switch
        {
            DungeonManager.BiomeEnum.Forest => _wildsProps,
            DungeonManager.BiomeEnum.Cave => _undergroundProps,
            DungeonManager.BiomeEnum.Dungeon => _dungeonProps,
            _ => null
        };
        if (biomePropTable != null && biomePropTable.Entries.Count > 0)
        {
            float propCount = Random.Range(1, 5) + Random.Range(1, 5); // 2d4
            propCount *= _dungeonManager.Data.RunLoop.Remap(1f, 3f, 1f, 1.25f);
            propCount *= ((int)_dungeonManager.Data.Biome).Remap(1f, 3f, 1f, 1.25f);
            for (int i = 0; i < propCount; i++)
            {
                var propPrefab = biomePropTable.SelectItem();
                Instantiate(propPrefab, SpawnSystem.GetRandomEmptyPos(), Quaternion.identity, _propHolder);
            }
        }
    }
}
