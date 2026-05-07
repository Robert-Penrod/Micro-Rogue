using System.Collections.Generic;
using UnityEngine;

public class SIE_FollowTarget : SIE
{
    [SerializeField] float _force = 5f;
    [SerializeField] float _range = 1f;
    Rigidbody2D _rb;
    List<Actor> _enemyList => _skillInstance.Skill.Actor?.Senses.EnemyActors;
    Transform _target;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    Transform GetTarget()
    {
        return _enemyList.Count > 0 ? _enemyList[0].transform : null;
    }

    private void FixedUpdate()
    {
        _target = GetTarget();
        Vector2 thisPos = _rb.position;
        Vector2 targetPos = _target?.position ?? thisPos;
        Vector2 dir = targetPos - thisPos;
        Vector2 followForce = dir.normalized * _force * dir.magnitude.Remap(_range, 1.5f * _range, 0f, 1f);
        //Debug.Log("Follow: " + followForce.magnitude);
        _rb.AddDampForce(followForce);
    }
}
