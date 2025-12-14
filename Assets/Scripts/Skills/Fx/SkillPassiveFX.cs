using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPassiveFX : MonoBehaviour
{
    [SerializeField] Transform _mainTransform;
    Skill _skill;
    SpriteRenderer _parentSpriteRend;
    ColorController _colorController;
    Actor _parentActor;
    Color _initParentColor;

    float _lerpAlpha;
    float _lerpScale;

    bool _isPlayer => _skill.Actor.CompareTag("Player");

    private void Start()
    {
        _skill = GetComponentInParent<Skill>();
        _parentActor = GetComponentInParent<Actor>();
        if(_parentActor != null) { _parentSpriteRend = GetComponentInParent<Actor>().GetComponentInChildren<SpriteRenderer>(); }
        _colorController = gameObject.GetOrAddComponent<ColorController>();
        UpdateSprites();

        this.DelayedInvoke(-1, () =>
        {
            _initParentColor = _parentSpriteRend.color;
        });
    }

    private void Update()
    {
        UpdateSprites();
    }

    void UpdateSprites()
    {
        // Alpha & scale
        float targetAlpha = 0f;
        float targetScale = 1f;
        float cooldownPercent = _skill.CooldownPercent;

        targetAlpha = cooldownPercent.Remap(0.5f, 1f, 0.35f, 1f);
        targetScale = cooldownPercent.Remap(0f, 1f, 0.7f, 1.2f);

        if (cooldownPercent < 0.9f)
        {
            targetAlpha *= 0.9f;
            targetScale *= 0.9f;
        }

        _lerpAlpha = _lerpAlpha.Lerp(targetAlpha, 12f * Time.deltaTime);
        _lerpScale = _lerpScale.Lerp(targetScale, 12f * Time.deltaTime);

        _colorController.SetColor(CalculatePassiveColor().Alpha(_lerpAlpha));
        _mainTransform.localScale = Vector3.one * _lerpScale;
    }

    Color CalculatePassiveColor()
    {
        Color passiveColor = new Color();
        int colorCount = 0;
        if (_parentSpriteRend != null)
        {
            Color parentColor = _parentSpriteRend.color;
            Color.RGBToHSV(parentColor, out float h, out float s, out float v);
            v -= 0.35f; // 0.43
            float min = 0.1f;
            if (v < min) v = min;
            h += 17f / 255f;
            passiveColor += Color.HSVToRGB(h, s, v);
            colorCount++;
        }

        if (_skill != null)
        {
            Color archetypalColor = _initParentColor;// GamePalette.Instance.GetColor(GetComponentInParent<Actor>()?.Faction == Actor.FactionType.Player, _skill.ArchetypalStats.Str, _skill.ArchetypalStats.Dex, _skill.ArchetypalStats.Int);
            passiveColor += Color.Lerp(passiveColor, archetypalColor, 0.25f);
            colorCount++;
        }

        if(colorCount != 0)
        {
            passiveColor /= colorCount;
        }
        else
        {
            passiveColor = Color.black;
        }

        Color skillColor = GamePaletteManager.I.Palette.GetSkillColor(_skill.Actor, _skill.Stats.Str, _skill.Stats.Dex, _skill.Stats.Int);
        passiveColor = passiveColor.Lerp(skillColor, _skill.CooldownPercent.Remap(0.5f, 1f, 0f, 0.75f));

        return passiveColor;
    }
}
