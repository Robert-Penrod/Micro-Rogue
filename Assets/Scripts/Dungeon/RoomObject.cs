using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField] GameObject _wildsRoomShapes;
    [SerializeField] GameObject _undergroundRoomShapes;
    [SerializeField] GameObject _dungeonRoomShapes;

    DungeonManager _dungeonManager;

    private void Start()
    {
        _dungeonManager = DungeonManager.I;
        Generate();
    }

    void Generate()
    {
        var biome = _dungeonManager.Data.Biome;
        _wildsRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Wilds);
        _undergroundRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Underground);
        _dungeonRoomShapes.SetActive(biome == DungeonManager.BiomeEnum.Dungeon);
    }
}
