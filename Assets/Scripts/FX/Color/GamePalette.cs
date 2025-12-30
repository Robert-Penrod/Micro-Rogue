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

    public Color GetSkillColor(Skill skill) => GetArchetypeColor(skill.Stats.Str, skill.Stats.Dex, skill.Stats.Int);
    public Color GetActorSkillColor(Skill skill) => GetActorSkillColor(skill.Actor, skill.Stats.Str, skill.Stats.Dex, skill.Stats.Int);
    public Color GetActorSkillColor(Actor actor, int str = 1, int dex = 1, int intel = 1)
    {
        Color individualPlayerColor = EnemyColor; // Player vs enemy color
        if(actor.IsPlayer())
        {
            individualPlayerColor = actor.GetComponentInParent<Player>().Data.Color;
            individualPlayerColor = individualPlayerColor.Lerp(PlayerColor, 0f);
        }
        else if(actor.Faction == Actor.FactionType.Player)
        {
            individualPlayerColor = PlayerColor;
        }
        Color archetypeColor = GetArchetypeColor(str, dex, intel).SetValue(1f);

        Color color = Color.Lerp(individualPlayerColor, archetypeColor, 0.25f); // 0.25f
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
