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

        //_mainTransform.gameObject.SetActive(false);
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

        if (_parentActor.IsInUI) cooldownPercent = 0.95f;

        //targetAlpha = cooldownPercent.Remap(0.1f, 1f, 0f, 1f); // 0.5f, 1f, 0.375f, 1f

        targetAlpha = cooldownPercent.Remap(0.5f, 1f, 0.375f, 1f) * cooldownPercent.Remap(0f, 0.5f, 0f, 1f);

        targetScale = cooldownPercent.Remap(0f, 1f, 0.7f, 1.2f);

        if (cooldownPercent < 0.9f)
        {
            targetAlpha *= 0.9f;
            targetScale *= 0.9f;
        }

        targetScale *= _skill.Stats.Size.Value;

        if(_skill.Slot == Skill.SlotEnum.Passive || _skill.Slot == Skill.SlotEnum.Item)
        {
            targetAlpha = targetScale = 1f;
        }

        _lerpAlpha = _lerpAlpha.Lerp(targetAlpha, 3f * Time.deltaTime);
        _lerpScale = _lerpScale.Lerp(targetScale, 3f * Time.deltaTime);

        float actorSpriteAlphaMult = _skill?.Actor?._spriteRend?.color.a ?? 1f;
        _lerpAlpha *= actorSpriteAlphaMult;

        _colorController.SetColor(CalculatePassiveColor().Alpha(_lerpAlpha));
        _mainTransform.localScale = Vector3.one * _lerpScale;
    }

    Color CalculatePassiveColor()
    {
        Color passiveColor = new Color();
        if (_parentSpriteRend != null)
        {
            Color parentColor = _parentSpriteRend.color;
            Color.RGBToHSV(parentColor, out float h, out float s, out float v);
            v -= 0.35f; // 0.43
            float min = 0.1f;
            if (v < min) v = min;
            h += 17f / 255f;
            passiveColor = Color.HSVToRGB(h, s, v);
        }

        Color skillColor = GamePaletteManager.I.Palette.GetActorSkillColor(_skill);
        float maxSkillColorLerp = _skill.Stats.Damage.Value > 0 ? 0.2f : 0f;
        float cooldownPercent = _skill.Slot == Skill.SlotEnum.Passive ? 1f : _skill.CooldownPercent;
        passiveColor = passiveColor.Lerp(skillColor, cooldownPercent.RemapPercent(0f, maxSkillColorLerp)).SetValue(0.7f);

        return passiveColor;
    }
}
