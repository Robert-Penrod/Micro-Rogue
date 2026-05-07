using System.Collections.Generic;
using UnityEngine;

public class SE_Shield : SkillEffect
{
    [SerializeField] SpriteRenderer _bashEffect;
    [SerializeField] AudioClip BlockSound;
    int _charges;
    float _bashRadius => 1f * _skill.Stats.Size.Value * _skill.Stats.Knockback.Value;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_skill.Actor.transform.position, _bashRadius);
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        // Charge Sheild
        _charges++;
        _charges = _charges.ClampInt(0, 1);// _charges.ClampInt(0, (int)_skill.Stats.Count.Value);
        if(_charges > 0)
        {
            SetShieldUp(true);
        }
    }

    private void Start()
    {
        _skill.Actor.OnWasHit += () =>
        {
            if (_charges <= 0) return;
            HandleShieldHit();
            Bash();
        };
    }

    float _bashEffectTick = float.MaxValue;
    private void Update()
    {
        if(_bashEffectTick < 1f)
        {
            // Tick
            if (!_bashEffect.enabled) _bashEffect.enabled = true;
            _bashEffectTick += Time.deltaTime * (1f / 0.25f);
            float intensity = _bashEffectTick < 0.25f? _bashEffectTick.Remap(0f, 0.25f, 0f, 1f) : _bashEffectTick.Remap(0.25f, 1f, 1f, 0f);
            
            // FX
            _bashEffect.color = _bashEffect.color.Alpha(intensity.RemapPercent(0f, 0.5f));
            _bashEffect.transform.localScale = _bashEffectTick.RemapPercent(0.5f, _bashRadius) *  Vector3.one;
        }
        else
        {
            if (_bashEffect.enabled) _bashEffect.enabled = false;
        }
    }

    void HandleShieldHit()
    {
        _charges -= 1;
        AudioSpawner.PlayAudioWithRandPitch(BlockSound, 0.2f, 1f, 1f, transform.position);
        if (_charges <= 0) SetShieldUp(false);
        _skill.ResetCooldown();
    }

    void SetShieldUp(bool isShieldUp)
    {
        string shieldTag = this.gameObject.name + " -Shield";
        var stat = _skill.Actor.Stats.Defense; 
        if (isShieldUp)
        {
            stat.AddModifier(new Kryz.Stats.StatModifier(2000, Kryz.Stats.StatModType.Flat, new List<string> { shieldTag }));
        }
        else
        {
            stat.RemoveAllModifiersWithTag(shieldTag);
        }
    }

    void Bash()
    {
        var thisActorPos = _skill.Actor.transform.position;
        var bashActorList = Utils.ComponentScan<Actor>(thisActorPos, _bashRadius);
        foreach(Actor actor in bashActorList)
        {
            if (actor == _skill.Actor) continue;
            Vector2 bashVector = actor.transform.position - thisActorPos;
            float dist = bashVector.magnitude;
            bashVector.Normalize();
            bashVector *= _skill.Stats.Knockback.Value;
            bashVector *= dist.Remap(0f, _bashRadius, 1f, 0f);
            actor.Body.AddDecayForce(bashVector);
        }

        _bashEffectTick = 0f;
    }
}
