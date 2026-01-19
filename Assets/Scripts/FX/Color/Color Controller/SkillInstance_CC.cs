using ManaSprite.EasyPooling;
using UnityEngine;

[RequireComponent(typeof(SkillInstance))]
public class SkillInstance_CC : ColorController, IPoolable
{
    [SerializeField] SpriteRenderer _mainSprite;
    SkillInstance _skillInstance;
    Skill _skill;
    Color _initColor;
    public bool UseInitColor = false;

    private void Awake()
    {
        _skillInstance = GetComponent<SkillInstance>();
        _skill = _skillInstance.Skill;
        _initColor = _mainSprite.color;
        UpdateColor();
    }

    public void Initialize()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        Color paletteColor;

        //paletteColor = paletteColor.Lerp(baseColor, 0.25f);

        if (UseInitColor) paletteColor = _initColor;
        else paletteColor = GamePaletteManager.I.Palette.GetActorSkillColor(_skill.Actor, _skill.Stats.Str, _skill.Stats.Dex, _skill.Stats.Int); 

        SetColor(paletteColor);
    }
}
