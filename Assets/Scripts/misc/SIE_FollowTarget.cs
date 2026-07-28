using ManaSprite.EasyPooling;
using System.Collections.Generic;
using UnityEngine;

public class SIE_FollowTarget : SIE, IPoolable
{
    [SerializeField] float _force = 5f;
    [SerializeField] float _range = 1f;
    [SerializeField] float _noiseFreq = 1f;
    [SerializeField] float _noiseMag = 0f;
    float _noiseTick = 0f;
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
        Init();
    }

    public void Initialize()
    {
        Init();
    }

    void Init()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
        _noiseTick = Random.Range(-1000f, 1000f);
    }

    Transform GetTarget()
    {
        for(int i = 0; i < _enemyList.Count; i++)
        {
            if (_skillInstance._previousTargetsList.Contains(_enemyList[i].gameObject)) continue;
            return _enemyList[i].transform;
        }
        return null;
    }

    private void FixedUpdate()
    {
        _target = GetTarget();
        Vector2 thisPos = _rb.position;
        Vector2 targetPos = _target?.position ?? thisPos;
        Vector2 dir = targetPos - thisPos;

        _noiseTick += _noiseFreq * Time.fixedDeltaTime;
        Vector2 noiseVector = _noiseMag * (new Vector2(Mathf.PerlinNoise(_noiseTick, 0.15f).RemapPercent(-1f, 1f), Mathf.PerlinNoise(_noiseTick, 42.51f).RemapPercent(-1f, 1f))).normalized;

        Vector2 followForce = noiseVector + dir.normalized * _force * dir.magnitude.Remap(_range, 1.5f * _range, 0f, 1f);

        //Debug.Log("Follow: " + followForce.magnitude);
        _rb.AddDampForce(followForce);
    }
}
