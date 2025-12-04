using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ActorSenses))]
public class Actor : MonoBehaviour
{
    [Header("Config")]
    public FactionType Faction = FactionType.Enemy;
    public enum FactionType { None, Player, Enemy }
    public ActorStats Stats;

    // Data
    public bool IsAlive { get; private set; }
    float _initScale;

    // References
    public Rigidbody2D Body { get; private set; }
    public ActorSenses Senses { get; private set; }

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Senses = GetComponent<ActorSenses>();

        // Init
        IsAlive = true;
        Stats.HealthMax.BaseValue = Stats.Health;
        Stats.SetHealthPercent(1f);

        // Scale
        _initScale = transform.localScale.x;
        transform.localScale = Vector3.zero;

        // Health Change
        Stats.OnHealthChanged += (float newHp, float deltaHp) =>
        {
            if (newHp <= 0 && IsAlive) Die();
        };
    }

    void Die()
    {
        gameObject.SetCollidersEnabled2D(false);
        IsAlive = false;

        this.DelayedInvoke(0.25f * Random.Range(0.9f, 1.1f), () =>
        {
            Destroy(this.gameObject);
        });
    }

    private void Update()
    {
        ScaleUpdate();
    }

    private void FixedUpdate()
    {
        HandleMoveFixedUpdate();
    }

    public void Rest()
    {
        Stats.Health += (int)(Stats.HealthMax.Value * 0.3f);
    }

    public bool IsPlayer()
    {
        return gameObject.CompareTag("Player");
    }

    public bool IsEnemyOf(Actor otherActor)
    {
        if (Faction == FactionType.None) return false;
        if (otherActor.Faction == FactionType.None) return false;
        if (otherActor.Faction == Faction) return false;

        return true;
    }

    public bool HasLineOfSightOf(Actor otherActor) => HasLineOfSightOf(otherActor.transform.position);
    public bool HasLineOfSightOf(Vector2 position)
    {
        Vector2 dir = position - (Vector2)transform.position;
        float dist = dir.magnitude;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.2f, dir, dist);
        foreach (RaycastHit2D hit in hits)
        {
            bool isTrigger = hit.collider.isTrigger;
            Actor hitActor = hit.collider.GetComponentInParent<Actor>();
            bool isActor = hitActor != null;
            bool isSkill = hit.collider.GetComponentInParent<Skill>() != null;

            if (!isTrigger && !isActor && !isSkill)
            {
                return false;
            }
        }
        return true;
    }

    #region Move
    public Vector2 MoveInput { get; private set; }
    float _moveTime;
    bool _moveRest;
    [SerializeField] bool _doMoveTime = true;
    public void Move(Vector2 move)
    {
        move = Vector2.ClampMagnitude(move, 1f);
        MoveInput = move;
    }

    void HandleMoveFixedUpdate()
    {
        StepMoveTime();

        float baseSpeed = Constants.ActorStats.MoveSpeed.Default;
        float movetimeMult = _doMoveTime ? _moveTime.Remap(5f, 10f, 1f, 0.8f) : 1f;

        Body.AddForce(movetimeMult * baseSpeed * MoveInput * Body.linearDamping);
    }

    void StepMoveTime()
    {
        if (!_doMoveTime) return;

        float moveSpeed = Body.linearVelocity.magnitude;
        float movePercent = moveSpeed.Remap(0f, 1f, 0f, 1f);

        // Moving
        if (!_moveRest && Body.linearVelocity.magnitude > 1f)
        {
            _moveTime += movePercent * Time.fixedDeltaTime;
            if (_moveTime > 10)
            {
                _moveTime = 10;
                _moveRest = true;
            }
        }
        // Resting
        else
        {
            _moveTime -= 4f * Time.fixedDeltaTime;
            if (_moveTime < 0)
            {
                _moveTime = 0;
                _moveRest = false;
            }
        }
    }
    #endregion

    #region Container
    bool _inPortal;
    public void SetInPortal(bool inPortal)
    {
        this._inPortal = inPortal;
    }
    void ScaleUpdate()
    {
        float targetS = _initScale;
        if (_inPortal) targetS *= 0.5f;

        float lerpS = Mathf.Lerp(transform.localScale.x, targetS, 12f * Time.deltaTime);
        transform.localScale = lerpS * Vector3.one;
    }
    #endregion
}
