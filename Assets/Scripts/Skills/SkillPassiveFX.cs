using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPassiveFX : MonoBehaviour
{
    Skill _skill;
    SpriteRenderer _parentSpriteRend;
    List<SpriteRenderer> _childSpriteRends = new List<SpriteRenderer>();
    Color _initParentColor;

    float _lerpAlpha;
    float _lerpScale;

    Dictionary<SpriteRenderer, float> _initScaleDict = new();

    private void Start()
    {
        _skill = GetComponentInParent<Skill>();
        Actor parentActor = GetComponentInParent<Actor>();
        if(parentActor != null) { _parentSpriteRend = GetComponentInParent<Actor>().GetComponentInChildren<SpriteRenderer>(); }       
        _childSpriteRends.AddRange(GetComponentsInChildren<SpriteRenderer>());
        UpdateSprites();

        _initParentColor = _parentSpriteRend.color;

        _childSpriteRends.ForEach(x =>
        {
            _initScaleDict.Add(x, x.transform.localScale.x);
        });
    }

    private void Update()
    {
        UpdateSprites();
        transform.localScale = Vector3.one * _skill.Stats.Size;
    }

    void UpdateSprites()
    {
        if (_childSpriteRends.Count > 0)
        {
            // Alpha & scale
            float targetAlpha = 0f;
            float targetScale = 1f;
            float cooldownPercent = _skill.CooldownPercent;

            targetAlpha = cooldownPercent.Remap(0.5f, 1f, 0.35f, 1f);
            targetScale = cooldownPercent.Remap(0f, 1f, 0.7f, 1.1f);

            if(cooldownPercent < 0.9f)
            {
                targetAlpha *= 0.9f;
                targetScale *= 0.9f;
            }

            _lerpAlpha = _lerpAlpha.Lerp(targetAlpha, 12f * Time.deltaTime);
            _lerpScale = _lerpScale.Lerp(targetScale, 12f * Time.deltaTime);

            _childSpriteRends.ForEach(childSpriteRend =>
            {
                childSpriteRend.color = CalculatePassiveColor().Alpha(_lerpAlpha);
                if(_initScaleDict.ContainsKey(childSpriteRend))
                    childSpriteRend.transform.localScale = _initScaleDict[childSpriteRend] * Vector3.one * _lerpScale;
            });
        }
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

        return passiveColor;
    }
}
