using UnityEngine;

public class UpgradeCountTextSetter : TMProSetter
{
    private void Update()
    {
        TextMesh.text = $"{DungeonManager.I.Data.UpgradeCount}";
    }
}
