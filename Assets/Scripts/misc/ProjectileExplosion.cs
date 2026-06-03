using ManaSprite.EasyPooling;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : SkillPart, IPoolable
{
    [SerializeField] AudioClip _explodeAudio;
    [SerializeField] AudioClip _damageAudio;
    [SerializeField] float _damageMult = 0.5f;
    [SerializeField] float _sizeMult = 1f;
    [SerializeField] float _explosionTime = 1f;
    float _explosionTick;

    [SerializeField] ParticleSystem _pSystem;
    float _radius => transform.localScale.x;

    float _knockback => SourceSkill.Stats.Knockback.Value;
    float _size => SourceSkill.Stats.Size.Value;

    List<Collider2D> _colCache = new();

    [SerializeField] ParticleSystem _hitParticles;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnEnable()
    {
        this.DelayedInvoke(0.1f, () =>
        {
            DoExplosion();
        });
    }

    public override void SetSourceSkillInstance(SkillInstance skillInstance)
    {
        base.SetSourceSkillInstance(skillInstance);
        var main = _pSystem.main;
        main.startColor = GamePaletteManager.I.Palette.GetActorSkillColor(SourceSkill).Alpha(main.startColor.color.a);
        transform.localScale = _sizeMult * _size * Vector3.one;

        _explosionTime = SourceSkill.Stats.Duration.Value;
        main.startLifetime = _explosionTime;
    }

    private void Update()
    {
        _explosionTick += Time.deltaTime;

        if(!_pSystem.isPlaying)
        {
            this.gameObject.DestroyOrRecycle();
        }
    }

    void DoExplosion()
    {
        /*
        Collider2D[] colArray = Physics2D.OverlapCircleAll(transform.position, _radius);
        foreach(Collider2D col in colArray)
        {
            HandleCollision(col);
        }
        */

        AudioSpawner.PlayAudioWithRandPitch(_explodeAudio, 0.2f, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_explosionTick / _explosionTime) > 0.25f) return;

        if (_colCache.Contains(collision)) return;
        _colCache.Add(collision);
        HandleCollision(collision);
    }

    void HandleCollision(Collider2D col)
    {
        float dist = Vector2.Distance(col.transform.position, this.transform.position);
        var body = col.GetComponent<Rigidbody2D>();
        var hitActor = col.GetComponent<Actor>();

        // Knockback
        if (body != null)
        {
            Vector2 dir = body.transform.position - transform.position;
            Vector2 knockbackVector = _size * _knockback * dir.normalized;// * dist.Remap(0f, _radius, 1f, 0f);
            // body.AddDampForce(knockbackVector, ForceMode2D.Impulse);
            //body.AddDecayForce(knockbackVector);
            if(hitActor != null)
            {
                hitActor.MoveController.ApplyKnockback(_knockback);
            }
            body.AddForce(knockbackVector * 5f, ForceMode2D.Impulse);
        }

        //  Damage
        if (hitActor != null)
        {
            if (!SourceSkill.Actor.IsEnemyOf(hitActor)) return;

            int damageTaken = hitActor.TakeDamage((int)(_damageMult * SourceSkill.Stats.CalculateDamageValue()), _skillInstance, null);

            if (damageTaken == 0) return;

            // Particles
            Vector2 vel = (hitActor.transform.position - transform.position);
            float mag = 1f;// vel.magnitude.Remap(_radius * 0.5f, _radius, 1f, 0.5f);
            vel = 7.5f * vel.normalized * mag;
            Vector2 pos = (transform.position + hitActor.transform.position) / 2f;
            HitParticlesManager.I.SpawnHitParticles(_skillInstance.Skill, pos, vel, mag);

            // Screen Shake
            CamShaker.I.Shake(Random.Range(0.2f, 0.3f), Random.Range(2.5f, 3.5f));

            // Audio
            float audioDelay = 0.125f * Random.Range(0.75f, 1.25f);
            float vol = damageTaken / _skillInstance.Skill.Stats.Damage.Value;
            AudioSpawner.PlayAudioWithRandPitch(_damageAudio, 0.2f, 1f, vol, delay: audioDelay);
        }
    }

    public void Initialize()
    {
        _colCache.Clear();
        _explosionTick = 0f;
    }
}
