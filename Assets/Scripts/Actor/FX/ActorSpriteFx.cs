using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActorSpriteFx : MonoBehaviour
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
                BleedPulse();
            }
        };
    }

    private void Start()
    {
        this.DelayedInvoke(-1, () =>
        {
            _initColor = _spriteRend.color;
        });
    }

    float _bleedTimer;
    void BleedPulse(float mag = 0.75f)
    {
        _bleedTimer += mag;
    }

    
    float _lerpMag = 0f;

    private void Update()
    {
        if (_initColor == null) return;

        // Init
        Color c = _initColor.Value;

        // Dodge
        float lerpT = _actor.MoveController.DodgeCooldownPercent;
        float alpha = 1f;
        if(_actor.MoveController.IsDodging)
        {
            lerpT -= 0.25f;
            alpha *= 0.25f; // 0.5f
        }
        if(_actor.MoveController.DodgeCooldownPercent < 0.99f)
        {
            lerpT -= 0.5f;
        }
        //dodgeLerp = dodgeLerp.Clamp01();
        Color dodgeColor = Color.Lerp(c, Color.black, lerpT.RemapPercent(0.375f, 0f));
        float lerpAlpha = c.a.Lerp(_initColor.Value.a * alpha, 500f * Time.deltaTime);
        c = dodgeColor.Alpha(lerpAlpha);


        // Bleed
        _bleedTimer -= Time.deltaTime;
        _bleedTimer = Mathf.Clamp(_bleedTimer, 0f, 1f);
        float lerpMult = _lerpMag < _bleedTimer ? 3f : 1f;
        _lerpMag = Mathf.Lerp(_lerpMag, _bleedTimer, lerpMult * 6f * Time.deltaTime);
        c = c.Lerp(Color.red, _lerpMag.Remap(0f, 1f, 0f, 0.8f));

        // Set color
        _spriteRend.color = c;
    }
}
