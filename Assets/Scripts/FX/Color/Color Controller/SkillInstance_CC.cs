using HyperQuest.EasyPooling;
using UnityEngine;

public class SkillInstance_CC : ColorController, IPoolable
{
    [SerializeField] SpriteRenderer _mainSprite;

    private void Awake()
    {
        UpdateColor();
    }

    public void Initialize()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        Color baseColor = _mainSprite?.color ?? Color.grey;

        bool isPlayer = true;
        Color paletteColor = GamePaletteManager.I.Palette.GetColor(isPlayer, 1, 0, 0);

        //paletteColor = paletteColor.Lerp(baseColor, 0.1f);

        SetColor(paletteColor);
    }
}
