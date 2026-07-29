using ManaSprite.EasyPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SIE_TelegraphAim : SIE, IPoolable
{
    [SerializeField] float _offsetAngle;

    List<Actor> _enemyList => _skillInstance?.Skill?.Actor?.Senses.EnemyActors;
    Actor _targetEnemy => _cachedTargetEnemy != null? _cachedTargetEnemy : ((_enemyList != null && _enemyList.Count > 0) ? _enemyList[0] : null);
    Actor _cachedTargetEnemy = null;
    float _cacheTargetTimer;
    float _speed => Constants.SkillStats.Speed.Default;

    float _angularVel;
    Vector2 _lerpAimPos;
    Vector2 _averageTargetVel;
    float _aimAheadRand = 1f;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        // Immediate Aim
        _cacheTargetTimer = 0f;

        if (this.enabled)
        {
            if (_targetEnemy != null) transform.up = transform.VectorTowards2D(_targetEnemy.transform);
            else transform.up = Random.insideUnitCircle.normalized;
        }
    }

    private void OnEnable()
    {
        //_aimAheadRand = Random.Range(0.25f, 1f);
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;

        //ManyAimAttempts();
        PrototypeAim();
        //Aim();
    }

    private void FixedUpdate()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;

        // Targeting
        _cacheTargetTimer += Time.fixedDeltaTime;
        if (_cacheTargetTimer >= 0.9f * _skillInstance.Skill.TelegraphTime)
        {
            _cachedTargetEnemy = _targetEnemy;
        }

        if(_targetEnemy == null)
        {
            //_skillInstance.StartPercent -= 1f * Time.fixedDeltaTime;
            //_skillInstance.ActivePercent -= 50f * Time.fixedDeltaTime;
        }
    }

    void Aim()
    {
        // INIT
        if (_targetEnemy == null)
        {
            _averageTargetVel = Vector2.zero;
            return;
        }
        Vector2 targetAimPos = _targetEnemy.transform.position;

        // PREDICTIVE OFFSET
        // Actor Vel Offset
        //targetAimPos -= 0.125f * _skillInstance.Skill.Actor.Body.linearVelocity;
        //
        // Target Vel Offset
        var targetBody = _targetEnemy.GetComponent<Rigidbody2D>();
        if (targetBody != null)
        {
            // Average Vel
            _averageTargetVel = _averageTargetVel.Lerp(targetBody.linearVelocity, 3f * Time.deltaTime);
            Debug.DrawLine(targetBody.transform.position, (Vector2)targetBody.transform.position + _averageTargetVel, Color.red.Lerp(Color.white, 0.5f));

            // Aim Ahead
            float dist = Vector2.Distance(transform.position, targetBody.transform.position);
            float mult = dist / _speed;
            float proximityMult = 1f;// dist.Remap(1f, 2f, 0.5f, 1f);
            float averageVelMult = _averageTargetVel.magnitude.Remap(0f, Constants.ActorStats.MoveSpeed.Default * 0.9f, 0f, 1f);
            targetAimPos += averageVelMult * proximityMult * mult * targetBody.linearVelocity;
        }

        Debug.DrawLine(transform.position, targetAimPos, Color.red);

        // AIM
        float torque = 16f; // 8  // 3, 2, 2.5
        float responseAngle = 180f;
        float damp = 4f; // 8  // 5, 10, 7.5, 8
        torque *= damp;

        float lerpSpeed = 20f;
        _lerpAimPos = _lerpAimPos.Lerp(targetAimPos, lerpSpeed * Time.deltaTime);
        Debug.DrawLine(transform.position, _lerpAimPos, Color.blue.Alpha(1f));


        Vector2 targetAimDir = _lerpAimPos - (Vector2)transform.position;
        float currentAngle = transform.rotation.eulerAngles.z;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        targetAngle += _offsetAngle;
        Utils.MotorDampAngle(currentAngle, targetAngle, ref _angularVel, torque, responseAngle, damp, Time.deltaTime);
        transform.Rotate2D(_skillInstance.Skill.Actor.FrostSlowMult * _angularVel);
    }

    void PrototypeAim()
    {
        float AimMult = 1f;

        AimMult *= _skillInstance.Skill.Actor.FrostSlowMult;
        AimMult *= _skillInstance.Skill.Actor.IsParalyzed ? 0f : 1f;

        float _aimLerp = 14f; // (12, 16) 6, 16, 8, 25
        if (_targetEnemy == null) return;
        Vector2 targetAimDir = _targetEnemy.transform.position - transform.position;
        float targetDist = targetAimDir.magnitude;

        // Vector2 actorAimDir = _targetEnemy.transform.position - _skillInstance.Skill.Actor.transform.position;
        // targetAimDir = (targetAimDir.normalized + actorAimDir.normalized * 0.25f).normalized;

        Rigidbody2D targetBody = _targetEnemy.GetComponent<Rigidbody2D>();
        if (targetBody != null)
        {
            _averageTargetVel = _averageTargetVel.Lerp(targetBody.linearVelocity, 3f * Time.deltaTime);
            float averageVelMult = _averageTargetVel.magnitude.Remap(0f, Constants.ActorStats.MoveSpeed.Default * 0.9f, 0f, 1f);
            float targetMult = _skillInstance.Skill.Actor.Faction == Actor.FactionType.Enemy ? 0.5f : 0.25f;
            float distMult = Mathf.Max(1f + ((targetDist - 1f) / 3f), 1f);
            Vector2 predictiveOffset = targetBody.linearVelocity * averageVelMult * targetMult * distMult;
            Vector2 predictivePos = (Vector2)_targetEnemy.transform.position + predictiveOffset;
            Vector2 predictiveAimDir = predictivePos - (Vector2)transform.position;

            // lerp predictive aim based on target dist
            float t = _aimAheadRand * targetDist.Remap(1f, 3f, 0f, 1f);
            targetAimDir = Vector2.Lerp(targetAimDir, predictiveAimDir, 0.5f * t);
        }

        // debug line
        Debug.DrawLine(transform.position, transform.position + (Vector3)targetAimDir * 5f);

        // Angle
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        targetAngle += -_offsetAngle * _skillInstance.Skill.GetDirection();
        float currentAngle = transform.rotation.eulerAngles.z;
        float lerpAngle = Mathf.LerpAngle(currentAngle, targetAngle, AimMult * _aimLerp * Time.deltaTime);

        // Rotation
        transform.rotation = Quaternion.Euler(0f, 0f, lerpAngle);
    }

    void ManyAimAttempts()
    {
        /*
        // 1
        if (_target == null) return;
        Vector2 aimPos = _target.transform.position;
        float distance = Vector2.Distance(transform.position, _target.transform.position);
        if(_targetBody != null)
        {
            aimPos += 0.25f * _targetBody.linearVelocity;
        }
        aimPos -= 0.5f * 0.25f * _skillInstance.Skill.Actor.Body.linearVelocity;

        Vector2 targetVector = aimPos - (Vector2)transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetVector);

        float deltaTime = _aimLerpSpeed * Time.deltaTime;
        deltaTime *= Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetAngle).Abs().Remap(0f, 180f, 1f, 0.1f);

        float lerpAngle = Mathf.LerpAngle(transform.localRotation.eulerAngles.z, targetAngle, deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, lerpAngle);
        */


        /*
        //2
        // Init
        if (_target == null) return;
        Vector2 targetAimPos = _target.transform.position;

        // Actor Vel Offset
        targetAimPos -= 0.125f * _skillInstance.Skill.Actor.Body.linearVelocity;

        // Target Vel Offset
        var targetBody = _target.GetComponent<Rigidbody2D>();
        if(targetBody != null)
        {
            //float mult = _skillInstance.Skill.Actor.Faction == Actor.FactionType.Player ? 0.25f : 0.375f;
            float mult = (Vector2.Distance(transform.position, targetBody.transform.position) - 1).ClampMin(0) / _speed;
            //float mult = 0.375f;
            targetAimPos += mult * targetBody.linearVelocity; // 0.125f
        }

        // Aim
        Vector2 targetAimDir = targetAimPos - (Vector2)transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        float currentAngle = transform.rotation.eulerAngles.z;
        float lerpAngle = Mathf.LerpAngle(currentAngle, targetAngle, _aimLerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, lerpAngle);
        */


        /*
        // 3
        // Init
        if (_target == null) return;
        Vector2 aimPos = _target.transform.position;
        float distance = Vector2.Distance(transform.position, _target.transform.position);

        // A
        if (_target.Body != null)
        {
            aimPos += (distance / _speed) * 1f * _target.Body.linearVelocity;
        }

        // Aim
        Vector2 targetVector = aimPos - (Vector2)transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetVector);
        float deltaTime = _aimLerpSpeed * Time.deltaTime;
        float lerpAngle = Mathf.LerpAngle(transform.localRotation.eulerAngles.z, targetAngle, deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, lerpAngle);
        */


        // Init
        /*
        if (_target == null) return;
        Vector2 targetAimPos = _target.transform.position;

        // Actor Vel Offset
        targetAimPos -= 0.125f * _skillInstance.Skill.Actor.Body.linearVelocity;

        // Target Vel Offset
        var targetBody = _target.GetComponent<Rigidbody2D>();
        if (targetBody != null)
        {
            //float mult = _skillInstance.Skill.Actor.Faction == Actor.FactionType.Player ? 0.25f : 0.375f;
            float mult = (Vector2.Distance(transform.position, targetBody.transform.position) - 1).ClampMin(0) / _speed;
            //float mult = 0.375f;
            //mult *= 0.5f;
            targetAimPos += mult * targetBody.linearVelocity; // 0.125f
        }

        // Aim
        Vector2 targetAimDir = targetAimPos - (Vector2)transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        float currentAngle = transform.rotation.eulerAngles.z;
        float deltaTime = _aimLerpSpeed * Time.deltaTime;
        //deltaTime *= (Vector2.Angle(transform.up, targetAimDir) / 180f).RemapPercent(1f, 0f); // Make changing targets harder with larger rotation
        //float lerpAngle = Mathf.LerpAngle(currentAngle, targetAngle, _aimLerpSpeed * deltaTime);
        //transform.rotation = Quaternion.Euler(0f, 0f, lerpAngle);

        float signedAngle = Mathf.DeltaAngle(currentAngle, targetAngle) / 180f;
        float angularAccel = signedAngle * Time.deltaTime;
        float drag = 6f;
        _angularVel += 6f * drag * angularAccel;
        drag *= signedAngle.Abs().Remap(0f, 1f, 1.5f, 1f);
        _angularVel = _angularVel * (1f - drag * Time.deltaTime);
        transform.Rotate2D(360f * _angularVel * Time.deltaTime);
        */

        // INIT
        if (_targetEnemy == null) return;
        Vector2 targetAimPos = _targetEnemy.transform.position;

        // PREDICTIVE OFFSET
        // Actor Vel Offset
        //targetAimPos -= 0.125f * _skillInstance.Skill.Actor.Body.linearVelocity;
        //
        // Target Vel Offset
        var targetBody = _targetEnemy.GetComponent<Rigidbody2D>();
        if (targetBody != null)
        {
            float dist = Vector2.Distance(transform.position, targetBody.transform.position);
            float mult = dist / _speed;
            float proximityMult = dist.Remap(1f, 2f, 0.5f, 1f);
            targetAimPos += proximityMult * mult * targetBody.linearVelocity;
        }

        Debug.DrawLine(transform.position, targetAimPos, Color.red);

        // AIM
        float torque = 8f; // 3, 2, 2.5
        float responseAngle = 180f;
        float damp = 8f; // 5, 10, 7.5, 8
        torque *= damp;
        _lerpAimPos = _lerpAimPos.Lerp(targetAimPos, 25f * Time.deltaTime);
        Debug.DrawLine(transform.position, _lerpAimPos, Color.blue);
        Vector2 targetAimDir = _lerpAimPos - (Vector2)transform.position;
        float currentAngle = transform.rotation.eulerAngles.z;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        targetAngle += _offsetAngle;
        Utils.MotorDampAngle(currentAngle, targetAngle, ref _angularVel, torque, responseAngle, damp, Time.deltaTime);
        transform.Rotate2D(_angularVel);
    }
}
