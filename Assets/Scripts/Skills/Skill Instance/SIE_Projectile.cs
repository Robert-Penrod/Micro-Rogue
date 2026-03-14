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

    List<Actor> _enemyList => _skillInstance?.Skill?.Actor?.Senses.EnemyActors;
    Actor _targetEnemy => _cachedTargetEnemy != null ? _cachedTargetEnemy : ((_enemyList != null && _enemyList.Count > 0) ? _enemyList[0] : null);
    Actor _cachedTargetEnemy = null;

    [SerializeField] AudioClip _hitClip;
    [SerializeField] AudioClip _wallHitClip;

    // Stats
    float _speed => _skillInstance.Skill.Stats.Speed.Value;
    float _hitboxDelay => _skillInstance.Skill.Stats.HitboxDelay;
    float _knockback => _knockbackMult * _skillInstance.Skill.Stats.Knockback.Value;
    float _pierce => _skillInstance.Skill.Stats.Pierce.Value;
    int _damage => (int)_skillInstance.Skill.Stats.Damage.Value;
    float _lunge => _skillInstance.Skill.Stats.Lunge.Value;
    float _piercePercent => _pierceCount.Remap(0f, _pierce, 0f, 1f);

    // References
    Rigidbody2D _rb;

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
            parentBody.AddDampForce(lungeForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if(_skillInstance.Skill.Actor == null)
        {
            _skillInstance.State = SkillInstance.SkillInstanceState.End;
        }

        if (_attached)
        {
            _rb.linearVelocity = _skillInstance?.Skill?.Actor?.Body?.linearVelocity ?? Vector2.zero;
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
    }

    private void OnCollisionEnter2D(Collision2D collision) => HandleCollisionStay(collision.collider, collision);
    private void OnCollisionStay2D(Collision2D collision) => HandleCollisionStay(collision.collider, collision);
    private void OnTriggerEnter2D(Collider2D collider) => HandleCollisionStay(collider);
    private void OnTriggerStay2D(Collider2D collider) => HandleCollisionStay(collider);
    void HandleCollisionStay(Collider2D col, Collision2D collision = null)
    {
        // State
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;

        float pierceMult = _skillInstance.ActivePercent < 0.01f? 0.5f : 1f;

        // References
        var hitActor = col.GetComponentInParent<Actor>();
        var skillInstance = col.GetComponentInParent<SkillInstance>();
        var hitBody = col.GetComponent<Rigidbody2D>();

        // Disable [Projectile <-> SkillInstance] Collisions
        if (skillInstance) return;

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
        //.

        // Hit Wall
        if (_hitsWalls && !hitActor && !col.isTrigger)
        {
            // Pierce
            _pierceCount += pierceMult * 0.5f;
            if(pierceMult > 0f) SlowProjectile(_wallSlowLerp);

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
            //damage *= _piercePercent.RemapPercent(1f, 0.75f);
            int damageTaken = hitActor.TakeDamage((int)damage, _skillInstance, null);

            // Pierce
            _pierceCount += 1f * pierceMult;
            SlowProjectile();

            // Hit Stun


            if (damageTaken > 0)
            {
                // Popup
                /*
                string colorString = "#" + ColorUtility.ToHtmlStringRGB(GamePaletteManager.I.Palette.GetActorSkillColor(_skillInstance.Skill).Lerp(Color.white, 0.25f));// hitActor.Faction == Actor.FactionType.Player ? "#FF9900" : "#FFFFFF";
                string popupString = "<color=" + colorString + ">-" + damageTaken.ToString() + "</color>";
                Vector3 popupPos = Vector2.Lerp(transform.position, hitActor.transform.position, hitActor.IsAlive ? 0.5f : 1f);
                popupPos += 0.25f * (Vector3)Random.insideUnitCircle;
                TextPopup2DManager.I.CreatePopup(popupPos, popupString, 0.5f * _rb.linearVelocity, hitActor.IsAlive ? hitActor.transform : null);
                */

                // Audio
                this.DelayedInvoke(0.02f, () =>
                {
                    PlayAudio(_hitClip, damageTaken / _skillInstance.Skill.Stats.Damage.Value);
                });
            }

            // Screen Shake
            CamShaker.I.Shake(Random.Range(0.2f, 0.3f), Random.Range(2.5f, 3.5f));
        }
        //.
        void SlowProjectile(float lerp = 1f)
        {
            if (_rb == null) return;
            _rb.linearVelocity *= 1f.Lerp(0.75f, lerp);
            _rb.angularVelocity *= 1f.Lerp(0.75f, lerp);
        }

        // Knockback
        if(hitBody != null)
        {
            Vector2 knockbackDir = _rb.linearVelocity.normalized;
            if (_rb.linearVelocity.sqrMagnitude < 0.1f) knockbackDir = transform.up.normalized;

            Vector2 knockbackForce = knockbackDir * _knockback;
            //knockbackForce *= hitBody.linearDamping;
            //knockbackForce *= _rb.linearVelocity.magnitude.Remap(0f, 8f, 0f, 1f, false).ClampMin(0f);

            //hitBody.AddForce(knockbackForce, ForceMode2D.Impulse);
            hitBody.AddDecayForce(knockbackForce);

            hitBody.transform.localScale *= 0.9f;
        }
        //.

        // Pierce end condition
        if (_pierce >= 0 && _pierceCount >= _pierce) _skillInstance.State = SkillInstance.SkillInstanceState.End;
    }

    void PlayAudio(AudioClip audio, float volMult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(audio, 0.2f, 1f, volMult * 0.5f, transform.position);
    }
}
