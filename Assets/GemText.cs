using UnityEngine;

public class GemText : TMProSetter
{
    private void Update()
    {
        TextMesh.text = Player.PlayerData.Gem.ToString();
    }
}
