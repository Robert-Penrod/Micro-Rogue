using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform _fill;
    [SerializeField] SpriteRenderer _fillDelta;
    [SerializeField] Transform _overhealFill;
    [SerializeField] Transform _armorFill;
    //[SerializeField] Transform _evadeFill;
    Actor _actor;
    Transform _gfx;
    float _targetHealthPercent = 1f;
    float _lerpSpeed = 1.5f;
    
    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _gfx = transform.GetChild(0);

        _actor.Stats.OnHealthChanged += (oldHP, newHP) => UpdateHealthPercent();
        _actor.Stats.HealthMax.OnValueChanged += (oldHp, newHo) => UpdateHealthPercent();

        _fillDelta.transform.localScale = new Vector3(1f, 0f, 1f);
        _fill.transform.localScale = new Vector3(1f, 1f, 1f);


        SetVisible(false);
    }

    void SetVisible(bool isVisible)
    {
        _gfx.gameObject.SetActive(isVisible);
    }

    void UpdateHealthPercent()
    {
        _targetHealthPercent = _actor.Stats.HealthPercent;
        /*
        if (_targetHealthPercent != 1f) SetVisible(true);
        else SetVisible(false);
        */
    }

    private void Update()
    {
        // Disable to simply use events
        UpdateHealthPercent();

        // Health
        float target = _targetHealthPercent.Clamp01();
        float current = _fill.transform.localScale.y;
        float dist = target - current;
        float distLerpMult = 1f / (5f * dist.Abs());
        float healthLerp = current.Lerp(target, distLerpMult * _lerpSpeed * Time.deltaTime);
        _fill.transform.localScale = new Vector3(1f, healthLerp, 1f);
        float delta = _targetHealthPercent.Clamp01() - healthLerp;
        float lerpDeltaMult = (healthLerp.Remap(0.9f, 1f, 0f, 1f)).RemapPercent(1f, 10f);
        float lerpDelta = _fillDelta.transform.localScale.y.Lerp(delta, lerpDeltaMult * 20f * _lerpSpeed * Time.deltaTime);
        _fillDelta.transform.localScale = new Vector3(1f, lerpDelta, 1f);
        _fillDelta.color = (delta > 0 ? Color.HSVToRGB(120 / 360f, 0.6f, 0.9f) : Color.red.Lerp(Color.yellow, 0.5f).SetSaturation(0.6f)).Alpha(_fillDelta.color.a);
        float overhealLerp = _overhealFill.transform.localScale.y.Lerp((_targetHealthPercent - 1f).Clamp01(), _lerpSpeed * Time.deltaTime);
        _overhealFill.transform.localScale = new Vector3(1f, overhealLerp, 1f);

        SetVisible(healthLerp < 0.99f);

        // Armor
        float armorPercent = ((_actor.Stats.Defense.Value + _actor.Stats.Evasion.Value) / (2f * (_actor.GetLevel() == 0? 1f : _actor.GetLevel()))).Clamp01();
        float armorLerp = _armorFill.transform.localScale.y.Lerp(armorPercent, _lerpSpeed * Time.deltaTime);
        _armorFill.transform.localScale = new Vector3(1f, armorLerp, 1f);

        // Evade
        /*
        float evadePercent = (_actor.Stats.Evasion.Value / _actor.Stats.HealthMax.Value).Clamp01();
        float evasionLerp = _evadeFill.transform.localScale.y.Lerp(evadePercent, _lerpSpeed * Time.deltaTime);
        _evadeFill.transform.localScale = new Vector3(1f, evasionLerp, 1f);
        */
    }
}
