using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BleedSpriteEffect : MonoBehaviour
{
    Actor _actor;
    SpriteRenderer _spriteRend;
    Color? _initColor = null;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _spriteRend = GetComponent<SpriteRenderer>();

        _actor.Stats.OnHealthChanged += (float newHp, float deltaHp) =>
        {
            if(deltaHp < 0)
            {
                HurtPulse();
            }
        };
    }

    private void Start()
    {
        this.DelayedInvoke(-1, () =>
        {
            _initColor = _spriteRend.color;
            SetEffectMagnitude(0f);
        });
    }

    void HurtPulse(float mag = 0.75f)
    {
        _effectCharge += mag;
    }

    void SetEffectMagnitude(float mag)
    {
        if (_initColor == null) return;

        Color c = _initColor.Value.Lerp(Color.red, mag.Remap(0f, 1f, 0f, 0.8f));
        _spriteRend.color = c;
    }

    float _effectCharge;
    float _lerpMag = 0f;

    private void Update()
    {
        _effectCharge -= 1f * Time.deltaTime;
        _effectCharge = Mathf.Clamp(_effectCharge, 0f, 1f);

        float lerpMult = _lerpMag < _effectCharge ? 3f : 1f;
        _lerpMag = Mathf.Lerp(_lerpMag, _effectCharge, lerpMult * 6f * Time.deltaTime);
        SetEffectMagnitude(_lerpMag);
    }
}
