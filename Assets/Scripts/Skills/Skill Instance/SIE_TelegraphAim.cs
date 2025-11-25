using HyperQuest.EasyPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SIE_TelegraphAim : SIE, IPoolable
{
    Transform _target;
    TickTimer _targetTimer = new TickTimer(0.1f);
    float _aimLerpSpeed = 10f;

    Rigidbody2D _targetBody;

    float _speed => 1f; // SkillInstance.GetSkillStat(SkillStats.SkillStatName.Speed).Value;

    

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        // Aim immediate
        _targetTimer.Reset();
        FindTarget();
        if (_target != null) transform.up = transform.VectorTowards2D(_target);
        else transform.up = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;
        HandleTargetScanning();
        Aim();
    }

    void Aim()
    {
        if (_target == null) return;
        Vector2 aimPos = _target.transform.position;
        float distance = Vector2.Distance(transform.position, _target.transform.position);
        if(_targetBody != null)
        {
            aimPos += (distance/_speed) * 1f * _targetBody.linearVelocity;
        }
        Vector2 targetVector = aimPos - (Vector2)transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.up, targetVector);

        float deltaTime = _aimLerpSpeed * Time.deltaTime;
        deltaTime *= Vector2.Dot(transform.up, targetVector).Remap(-1f, 1f, 0.25f, 1f); // Make changing targets harder with larger rotation

        float lerpAngle = Mathf.LerpAngle(transform.localRotation.eulerAngles.z, targetAngle, deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, lerpAngle);
    }

    void HandleTargetScanning()
    {
        _targetTimer.Tick(Time.deltaTime);
        if (_targetTimer.IsDone())
        {
            _targetTimer.Reset();
            FindTarget();
        }
    }

    void FindTarget()
    {
        
        List<Actor> enemyList = Utils.ComponentScan<Actor>(transform.position, 25f).FindAll(x => x.IsEnemyOf(_skillInstance.Skill.GetComponentInParent<Actor>()));
        if(enemyList.Count > 0)
        {
            if (_target != null && _target != enemyList[0].transform)
            {
                // Establishing new target
            }

            _target = enemyList[0].transform;
            _targetBody = enemyList[0].GetComponent<Rigidbody2D>();
        }
        else
        {
            _target = null;
        }
    }
}
