using UnityEngine;

public class LevelTextUI : TMProSetter
{
    private void Start()
    {
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            TextMesh.text = DungeonManager.I.Data.RoomNumber.ToString();
        };
    }
}
