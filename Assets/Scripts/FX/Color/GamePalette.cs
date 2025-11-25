using UnityEngine;


[CreateAssetMenu(menuName = "Color/GamePalette", fileName = "GamePalette")]
public class GamePalette : ScriptableObject
{
    [Header("Factions")]
    public Color PlayerColor;
    public Color EnemyColor;

    [Header("Stats")]
    public Color StrColor;
    public Color DexColor;
    public Color IntColor;

    public Color GetColor(bool isPlayer = false, int str = 1, int dex = 1, int intel = 1)
    {
        Color actorColor = isPlayer ? PlayerColor : EnemyColor; // Player vs enemy color
        Color archetypeColor = GetArchetypeColor(str, dex, intel);

        Color color = Color.Lerp(actorColor, archetypeColor, 0.25f);
        return color;
    }

    public Color GetArchetypeColor(int str, int dex, int intel)
    {
        float statTotal = Mathf.Pow(str, 2) + Mathf.Pow(dex, 2) + Mathf.Pow(intel, 2);
        float strPercent = Mathf.Pow(str, 2f) / statTotal;
        float dexPercent = Mathf.Pow(dex, 2f) / statTotal;
        float intelPercent = Mathf.Pow(intel, 2f) / statTotal;
        Color archetypeColor = (strPercent * StrColor + dexPercent * DexColor + intelPercent * IntColor);
        return archetypeColor;
    }
}
