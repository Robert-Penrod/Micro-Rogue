using TMPro;
using UnityEngine;

public class MapSelectionUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] TextMeshProUGUI DescriptionText;

    private void Start()
    {
        UpdateUI();
        DungeonManager.I.OnGenerateLevel += () =>
        {
            UpdateUI();
        };
    }

    void UpdateUI()
    {
        var data = DungeonManager.I.Data;
        TitleText.text = data.Biome.ToString();
        DescriptionText.text = data.Biome switch
        {
            DungeonManager.BiomeEnum.Forest => "A green wilderness full of faries and spiders and goblins, oh my!",
            DungeonManager.BiomeEnum.Cave => "Damp underground tunnels of rock punctuated by squeeks and skitters hidden in the dark...",
            DungeonManager.BiomeEnum.Dungeon => "Ruined labrynth holding arcane mysteries...",
        };
    }
}