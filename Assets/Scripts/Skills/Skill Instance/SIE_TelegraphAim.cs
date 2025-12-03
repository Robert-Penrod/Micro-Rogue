using HyperQuest.EasyPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SIE_TelegraphAim : SIE, IPoolable
{
    List<Actor> _enemyList => _skillInstance?.Skill?.Actor?.Senses.EnemyActors;
    Actor _target => (_enemyList != null && _enemyList.Count > 0) ? _enemyList[0] : null;
    TickTimer _targetTimer = new TickTimer(0.1f);
    float _aimLerpSpeed = 6f;

    Rigidbody2D _targetBody;

    float _speed => 1f; // SkillInstance.GetSkillStat(SkillStats.SkillStatName.Speed).Value;

    float _scanTime => 3f * Constants.SkillStats.BaseTelegraphTime;
    float _scanTick = 0f;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        // Aim immediate
        _targetTimer.Reset();
        if (_target != null) transform.up = transform.VectorTowards2D(_target.transform);
        else transform.up = Random.insideUnitCircle.normalized;

        _scanTick = 0f;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;
        Aim();
    }

    void Aim()
    {
        /*
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

        // Init
        if (_target == null) return;
        Vector2 targetAimPos = _target.transform.position;

        // Actor Vel Offset
        targetAimPos -= 0.125f * _skillInstance.Skill.Actor.Body.linearVelocity;

        // Target Vel Offset
        var targetBody = _target.GetComponent<Rigidbody2D>();
        if(targetBody != null)
        {
            targetAimPos += 0.25f * targetBody.linearVelocity; // 0.125f
        }

        // Aim
        Vector2 targetAimDir = targetAimPos - (Vector2)transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetAimDir);
        float currentAngle = transform.rotation.eulerAngles.z;
        float lerpAngle = Mathf.LerpAngle(currentAngle, targetAngle, _aimLerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, lerpAngle);
    }
}
