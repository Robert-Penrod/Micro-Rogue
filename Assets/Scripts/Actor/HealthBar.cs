using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform _fill;
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

        if(!IsLerpComplete())
        {
            float currentValue = _fill.transform.localScale.y;
            float lerpValue = Mathf.Lerp(currentValue, _targetHealthPercent, _lerpSpeed * Time.deltaTime);
            _fill.transform.localScale = new Vector3(1f, lerpValue, 1f);

            if(IsLerpComplete())
            {
                _fill.transform.localScale = new Vector3(1f, _targetHealthPercent, 1f);
            }
        }
    }

    bool IsLerpComplete()
    {
        return Mathf.Abs(_targetHealthPercent - _fill.transform.localScale.y) <= 0.01f;
    }
}
