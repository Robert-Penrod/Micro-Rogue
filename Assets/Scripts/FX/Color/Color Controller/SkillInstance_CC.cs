using ManaSprite.EasyPooling;
using UnityEngine;

[RequireComponent(typeof(SkillInstance))]
public class SkillInstance_CC : ColorController, IPoolable
{
    [SerializeField] SpriteRenderer _mainSprite;
    SkillInstance _skillInstance;
    Color _initColor;
    public bool UseInitColor = false;

    Skill _skill;

    private void Awake()
    {
        _skillInstance = GetComponent<SkillInstance>();
        _initColor = _mainSprite.color;
        UpdateColor();
    }

    public void Initialize()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        if (_skillInstance == null || _skillInstance.Skill == null) return;

        Color paletteColor;

        //paletteColor = paletteColor.Lerp(baseColor, 0.25f);
        if (UseInitColor) paletteColor = _initColor;
        else
        {
            _skill = _skillInstance.Skill;
            paletteColor = GamePaletteManager.I.Palette.GetActorSkillColor(_skill);
        }

        //float actorSpriteAlpha = _skill.Actor?._spriteRend?.color.a ?? 1f;
        //paletteColor.Alpha(paletteColor.a * actorSpriteAlpha);

        SetColor(paletteColor);
    }
}
