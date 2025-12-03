using HyperQuest.EasyPooling;
using UnityEngine;

[RequireComponent(typeof(SkillInstance))]
public class SkillInstance_CC : ColorController, IPoolable
{
    [SerializeField] SpriteRenderer _mainSprite;
    SkillInstance _skillInstance;
    Skill _skill;

    bool _isPlayer => _skillInstance.Skill.Actor.CompareTag("Player");

    private void Awake()
    {
        _skillInstance = GetComponent<SkillInstance>();
        _skill = _skillInstance.Skill;
        UpdateColor();
    }

    public void Initialize()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        Color baseColor = _mainSprite?.color ?? Color.grey;

        Color paletteColor = GamePaletteManager.I.Palette.GetColor(_skill.Actor, _skill.Stats.Str, _skill.Stats.Dex, _skill.Stats.Int);

        //paletteColor = paletteColor.Lerp(baseColor, 0.1f);

        SetColor(paletteColor);
    }
}
