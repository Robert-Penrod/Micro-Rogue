using System.Collections.Generic;
using UnityEngine;

public class SIE_FollowTarget : SIE
{
    [SerializeField] float _force;
    Rigidbody2D _rb;
    List<Actor> _enemyList => _skillInstance.Skill.Actor?.Senses.EnemyActors;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
    }

    Transform GetTarget()
    {
        return _enemyList.Count > 0 ? _enemyList[0].transform : null;
    }

    private void FixedUpdate()
    {
        Vector2 thisPos = _rb.position;
        Vector2 targetPos = GetTarget()?.position ?? thisPos;
        Vector2 dir = targetPos - thisPos;
        Vector2 followForce = dir * _force;
        _rb.AddDampForce(followForce);
    }
}
