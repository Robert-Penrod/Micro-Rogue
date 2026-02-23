using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform _fill;
    [SerializeField] Transform _armorFill;
    //[SerializeField] Transform _evadeFill;
    Actor _actor;
    Transform _gfx;
    float _targetHealthPercent = 1f;
    float _lerpSpeed = 6f;
    
    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _gfx = transform.GetChild(0);

        _actor.Stats.OnHealthChanged += (oldHP, newHP) => UpdateHealthPercent();
        _actor.Stats.HealthMax.OnValueChanged += (oldHp, newHo) => UpdateHealthPercent();

        SetVisible(false);
    }

    void SetVisible(bool isVisible)
    {
        _gfx.gameObject.SetActive(isVisible);
    }

    void UpdateHealthPercent()
    {
        _targetHealthPercent = _actor.Stats.HealthPercent;
        if (_targetHealthPercent < 1f) SetVisible(true);
        else SetVisible(false);
    }

    private void Update()
    {
        // Disable to simply use events
        UpdateHealthPercent();

        // Health
        float healthLerp = _fill.transform.localScale.y.Lerp(_targetHealthPercent, _lerpSpeed * Time.deltaTime);
        _fill.transform.localScale = new Vector3(1f, healthLerp, 1f);

        // Armor
        float armorPercent = ((_actor.Stats.Defense.Value + _actor.Stats.Evasion.Value) / _actor.Stats.HealthMax.Value).Clamp01();
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
