using ManaSprite.EasyPooling;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SIE_Projectile : SIE, IPoolable
{
    [SerializeField] bool _attached;
    [SerializeField] float _damageMult = 1f;
    [SerializeField] float _knockbackMult = 1f;
    [SerializeField] float _launchMult = 1f;
    [SerializeField] float _angularVelMult = 0f;
    [SerializeField] bool _hitsWalls = true;
    [SerializeField] float _wallSlowLerp = 0.5f;
    [SerializeField] ParticleSystem _hitParticles;
    [SerializeField] SkillInstance _spawnOnHit;
    [SerializeField] float _spawnOnHitChance = 0f;

    List<Actor> _enemyList => _skillInstance?.Skill?.Actor?.Senses.EnemyActors;
    Actor _targetEnemy => _cachedTargetEnemy != null ? _cachedTargetEnemy : ((_enemyList != null && _enemyList.Count > 0) ? _enemyList[0] : null);
    Actor _cachedTargetEnemy = null;

    [SerializeField] AudioClip _hitClip;
    [SerializeField] AudioClip _wallHitClip;

    // Stats
    float _speed => _skillInstance.Skill.Stats.Speed.Value;
    float _hitboxDelay => Constants.SkillStats.HitboxDelay;
    float _knockback => _knockbackMult * _skillInstance.Skill.Stats.Knockback.Value;
    float _pierce => _skillInstance.Skill.Stats.Pierce.Value;
    int _damage => (int)_skillInstance.Skill.Stats.Damage.Value;
    float _lunge => _skillInstance.Skill.Stats.Lunge.Value;
    float _piercePercent => _pierceCount.Remap(0f, _pierce, 0f, 1f);

    // References
    Rigidbody2D _rb;
    Vector2 _lastVel;

    // Data
    float _pierceCount;
    Dictionary<Collider2D, float> _colDict = new();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white.Alpha(0.5f);
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _skillInstance.OnActivated += Launch;
        _skillInstance.OnEnd += () =>
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        };
    }

    public void Initialize()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _pierceCount = 0;
        _colDict.Clear();
    }

    void Launch()
    {
        if (!this.enabled) return;
        //Debug.Log("Launch");
        transform.SetParent(null);
        gameObject.SetCollidersEnabled2D(true);
        _rb.bodyType = RigidbodyType2D.Dynamic;

        Vector2 launchForce = _speed * transform.up;

        var actor = _skillInstance.Skill.Actor;

        // Speed Inheritance
        Rigidbody2D parentBody = _skillInstance.ParentBody ?? actor?.Body;
        if (parentBody != null)
        {
            // Launch Force
            Vector2 projectedParentVel = Vector3.Project(parentBody.linearVelocity, launchForce.normalized);
            Vector2 inheritVel = projectedParentVel;
            //inheritVel *= Constants.SkillStats.SIE_ProjectileInheritVelocityMult;

            inheritVel *= Vector2.Dot(parentBody.linearVelocity, launchForce) > 0 ? 1f : 0.5f;

            launchForce += inheritVel;

            // Min Launch Force
            /*
            float minLaunchForce = _speed / 2f;
            if(launchForce.magnitude < minLaunchForce || Vector2.Dot(launchForce, transform.up) < 0)
            {
                launchForce = minLaunchForce * transform.up;
            }
            */
        }

        // Add Force
        float speed = 0.2f * 360f * _skillInstance.Skill.Stats.Speed.Value;
        _rb.AddForce(_launchMult * launchForce, ForceMode2D.Impulse);
        if (_angularVelMult.Abs() > 0)
        {
            _rb.angularVelocity = -_angularVelMult * speed * _skillInstance.Skill.GetDirection(true);
        }

        // Lunge
        if (parentBody != null)
        {
            Vector2 lungeForce = transform.up * _lunge;
            //parentBody.AddDampForce(lungeForce, ForceMode2D.Impulse); olf "bad physics" way
            parentBody.AddForce(5f * lungeForce, ForceMode2D.Impulse);
            _skillInstance.Skill.Actor.MoveController.ApplyKnockback(1f * _lunge);
        }
    }

    private void FixedUpdate()
    {
        if(_skillInstance.Skill.Actor == null)
        {
            _skillInstance.State = SkillInstance.SkillInstanceState.End;
        }

        if (_attached && _rb != null)
        {
            Vector2 vel = Vector2.zero;
            if(_skillInstance?.Skill?.Actor?.Body != null)
            {
                vel = _skillInstance.Skill.Actor.Body.linearVelocity;
            }
            _rb.linearVelocity = vel;
        }

        if (_rb == null) return;

        // Aim
        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Vector2.SignedAngle(Vector2.up, _rb.linearVelocity);
            float lerpAngle = _rb.rotation.LerpAngle(targetAngle, 12f * Time.deltaTime);
            //_rb.MoveRotation(lerpAngle);


            // Homing
            /*
            if (_cachedTargetEnemy == null) _cachedTargetEnemy = _targetEnemy;
            if (_targetEnemy == null) return;
            Vector2 currentDir = _rb.linearVelocity.normalized;
            Vector2 targetDir = (_targetEnemy.transform.position - transform.position).normalized;
            float currentAngle = Vector2.SignedAngle(Vector2.up, currentDir);
            float targetHomingAngle = Vector2.SignedAngle(Vector2.up, targetDir);
            float lerpHomingAngle = Mathf.LerpAngle(currentAngle, targetHomingAngle, _homing * Time.fixedDeltaTime);
            Vector2 lerpDir = Quaternion.Euler(0f, 0f, lerpHomingAngle) * Vector2.up;
            Vector2 newVel = lerpDir * _rb.linearVelocity.magnitude;
            _rb.linearVelocity = newVel;
            */
        }

        _lastVel = _rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision) => HandleCollisionStay(collision.collider, collision);
    private void OnCollisionStay2D(Collision2D collision) => HandleCollisionStay(collision.collider, collision);
    private void OnTriggerEnter2D(Collider2D collider) => HandleCollisionStay(collider);
    private void OnTriggerStay2D(Collider2D collider) => HandleCollisionStay(collider);
    void HandleCollisionStay(Collider2D col, Collision2D collision = null)
    {
        Vector2 particleVel = _lastVel * 0.25f;
        Vector2 particlePoint = collision != null ? collision.contacts[0].point : transform.position;

        // State
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;
        int damageDone = 0;
        bool didDodge = false;
        float startupPierceMult = _skillInstance.ActivePercent < 0.02f? 0f : 1f;

        // References
        var hitActor = col.GetComponentInParent<Actor>();
        var skillInstance = col.GetComponentInParent<SkillInstance>();
        var hitBody = col.GetComponent<Rigidbody2D>();

        // Disable [Projectile <-> SkillInstance] Collisions
        if (skillInstance) return;

        if (!_attached && _hitsWalls && !hitActor && !col.isTrigger)
        {
            _pierceCount += 0.5f;
            if (startupPierceMult > 0f) SlowProjectile(_wallSlowLerp);
        }

        // Hitbox Delay Check
        if (!PassesHitboxDelay(col)) { return; }
        bool PassesHitboxDelay(Collider2D c)
        {
            float hitboxDelay = _hitboxDelay;
            if (_colDict.ContainsKey(c) && Time.time - _colDict[c] < hitboxDelay) { return false; }
            else if (_colDict.ContainsKey(c)) { _colDict.Remove(c); }
            _colDict.Add(c, Time.time);
            return true;
        }

        // Hit Wall
        if (_hitsWalls && !hitActor && !col.isTrigger)
        {
            // Pierce
            _pierceCount += startupPierceMult * 0.5f;
            if(startupPierceMult > 0f) SlowProjectile(_wallSlowLerp);

            //HandleParticles(particlePoint, particleVel, 0.5f);

            // Audio
            PlayAudio(_wallHitClip, 0.25f);
        }
        // Hit Actor
        else if(hitActor)
        {
            // Disable friendly fire
            if (!hitActor.IsEnemyOf(_skillInstance.Skill.Actor)) return;

            // Damage
            float damage = (int)(_damageMult * _skillInstance.Skill.Stats.CalculateDamageValue());
            bool didCrit = _skillInstance.Skill.Stats.RollForCrit();
            if (didCrit) damage *= 1.5f;
            //damage *= _piercePercent.RemapPercent(1f, 0.75f);
            damageDone = hitActor.TakeDamage(damage, _skillInstance, null, isCrit: didCrit);
            didDodge = hitActor.MoveController.IsDodging;

            if (!didDodge)
            {
                // Pierce
                _pierceCount += 1f;// * pierceMult;
                SlowProjectile();
            }            

            if (damageDone > 0)
            {
                // Audio
                this.DelayedInvoke(0.02f, () =>
                {
                    PlayAudio(_hitClip, damageDone / _skillInstance.Skill.Stats.Damage.Value);
                });

                particlePoint = particlePoint.Lerp(hitActor.transform.position, 0.75f);
                float particleMag = damageDone.Remap(0f, 0.25f * hitActor.Stats.HealthMax.Value, 0f, 1f);
                HandleParticles(particlePoint, particleVel, particleMag);
            }

            // Events
            var skill = _skillInstance.Skill;
            skill.OnHit?.Invoke(damageDone, hitActor, skill);
            if (hitActor.Stats.Health < 0f) skill.OnKill?.Invoke(hitActor);

            // Spawn on Hit
            if(!didDodge && _spawnOnHit != null && Random.value < _spawnOnHitChance)
            {
                var spawnedSkillInstance = Instantiate(_spawnOnHit).GetComponent<SkillInstance>();
                spawnedSkillInstance.transform.position = (Vector3)transform.position + Vector3.forward * spawnedSkillInstance.transform.position.z;
                spawnedSkillInstance.Skill = skill;
                spawnedSkillInstance.gameObject.SetActive(true);
                spawnedSkillInstance._previousTargetsList.Add(hitActor.gameObject);
            }
        }
        //.
        void SlowProjectile(float lerp = 1f)
        {
            if (_rb == null) return;
            _rb.linearVelocity *= 1f.Lerp(0.75f, lerp);
            _rb.angularVelocity *= 1f.Lerp(0.875f, lerp);
        }

        // Knockback
        if(hitBody != null && hitBody.bodyType == RigidbodyType2D.Dynamic && (hitActor == null || !didDodge))
        {
            Vector2 knockbackVel = _rb.linearVelocity;
            if (_rb.linearVelocity.sqrMagnitude < 0.1f) knockbackVel = transform.up.normalized;
            knockbackVel = transform.up.normalized;
            Vector2 knockbackForce = knockbackVel * _knockback;
            if(hitActor != null)
            {
                hitActor.MoveController.ApplyKnockback(0.75f * _knockback);
            }
            //hitBody.AddDecayForce(knockbackForce, _knockback);
            hitBody.AddForce(7.5f * knockbackForce, ForceMode2D.Impulse); // 5f

            // Screen Shake
            float screenShakeMult = 1f;
            float amp = Random.Range(0.2f, 0.3f);
            float freq = Random.Range(2.5f, 3.5f);
            if (hitActor != null) screenShakeMult *= damageDone.Remap(0f, 0.25f * hitActor.Stats.HealthMax.Value, 0.25f, 1f);
            screenShakeMult *= _knockback.Abs().Remap(0f, 2f, 0.25f, 1f);
            amp *= screenShakeMult;
            freq *= screenShakeMult;
            Vector2 direction = (hitActor.transform.position - this.transform.position).normalized;
            CamShaker.I.Shake(amp, freq, direction);
        }

        // Pierce end condition
        float fractionalPierce = _pierce - (int)_pierce;
        float roll = Random.value;
        float finalPierceCheck = (int)_pierce + ((roll < fractionalPierce) ? 1 : 0);
        if(_pierceCount > 0) Debug.Log($"FractionalPierce: {roll} < {fractionalPierce}? -> {_pierceCount} / {finalPierceCheck}");
        if (_pierce >= 0 && _pierceCount > 0 && _pierceCount >= finalPierceCheck) _skillInstance.State = SkillInstance.SkillInstanceState.End;
    }

    void HandleParticles(Vector2 hitPos, Vector2 hitVel, float mult = 1f)
    {
        HitParticlesManager.I.SpawnHitParticles(_skillInstance.Skill, hitPos, hitVel, mult);
        /*
        var pSystem = Instantiate(_hitParticles).GetComponent<ParticleSystem>();
        pSystem.transform.position = (Vector3)hitPos + Vector3.forward * _hitParticles.transform.position.z;
        var main = pSystem.main;
        main.startColor = GamePaletteManager.I.Palette.GetActorSkillColor(_skillInstance.Skill).Lerp(Color.white, 0.1f).Alpha(0.5f * mult);

        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f * mult, 5f * mult);

        var velOverLifetime = pSystem.velocityOverLifetime;
        velOverLifetime.enabled = true;
        velOverLifetime.x = hitVel.x;
        velOverLifetime.y = hitVel.y;

        var emission = pSystem.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(2 * mult), (short)(3 * mult)));

        pSystem.Play();
        */
    }

    void PlayAudio(AudioClip audio, float volMult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(audio, 0.2f, 1f, volMult * 0.5f, transform.position);
    }
}
