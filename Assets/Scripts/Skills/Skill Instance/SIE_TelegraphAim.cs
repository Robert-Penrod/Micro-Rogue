using ManaSprite.EasyPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SIE_TelegraphAim : SIE, IPoolable
{
    List<Actor> _enemyList => _skillInstance?.Skill?.Actor?.Senses.EnemyActors;
    Actor _targetEnemy => _cachedTargetEnemy != null? _cachedTargetEnemy : ((_enemyList != null && _enemyList.Count > 0) ? _enemyList[0] : null);
    Actor _cachedTargetEnemy = null;
    TickTimer _targetTimer = new TickTimer(0.2f);
    float _speed => Constants.SkillStats.Speed.Default;

    float _angularVel;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        // Immediate Aim
        _targetTimer.Reset();
        if (_targetEnemy != null) transform.up = transform.VectorTowards2D(_targetEnemy.transform);
        else transform.up = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;
        _targetTimer.Tick(Time.deltaTime);
        if(_targetTimer.IsDone())
        {
            _cachedTargetEnemy = _targetEnemy;
        }
        Aim();
    }

    void Aim()
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
        targetAimPos -= 0.125f * _skillInstance.Skill.Actor.Body.linearVelocity;
        //
        // Target Vel Offset
        var targetBody = _targetEnemy.GetComponent<Rigidbody2D>();
        if (targetBody != null)
        {
            float mult = Vector2.Distance(transform.position, targetBody.transform.position) / _speed;
            targetAimPos += mult * targetBody.linearVelocity;
        }

        // AIM
        float torque = 2.5f; // 3, 2
        float responseAngle = 180f;
        float damp = 8f; // 5, 10, 7.5
        torque *= damp;
        Vector2 targetAimDir = targetAimPos - (Vector2)transform.position;
        float currentAngle = transform.rotation.eulerAngles.z;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        Utils.MotorDampAngle(currentAngle, targetAngle, ref _angularVel, torque, responseAngle, damp, Time.deltaTime);
        transform.Rotate2D(_angularVel);
    }
}
