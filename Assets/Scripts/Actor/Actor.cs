using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    [Header("Config")]
    public FactionType Faction = FactionType.Enemy;
    public enum FactionType { None, Player, Enemy }
    public ActorStats Stats;

    // Data
    public bool IsAlive { get; private set; }
    float _initScale;
    float _initialLinearDamping;

    // References
    public Rigidbody2D Body { get; private set; }
    public ActorMoveController MoveController { get; private set; }
    public ActorSenses Senses { get; private set; }
    public ActorSkillSystem SkillSystem { get; private set; }

    // Events
    public Action OnUpgrade;
    public Action OnDeath;

    [SerializeField] SpriteRenderer _spriteRend;
    public Sprite Sprite => _spriteRend.sprite;

    public int GetLevel()
    {
        if (SkillSystem == null) return 0;

        int level = 0;
        SkillSystem.SkillList.ForEach(skill =>
        {
            level += skill.Level;
        });
        return level;
    }

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        _initialLinearDamping = Body.linearDamping;
        MoveController = GetComponent<ActorMoveController>();
        Senses = GetComponent<ActorSenses>();
        SkillSystem = GetComponentInChildren<ActorSkillSystem>();

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

        if (!IsPlayer())
        {
            this.DelayedInvoke(0.25f * Random.Range(0.9f, 1.1f), () =>
            {
                Destroy(this.gameObject);
            });
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        OnDeath?.Invoke();
    }

    public bool IsInUI { get; private set; }
    Vector3 _preUIPos;
    private void Update()
    {
        ScaleUpdate();
    }

    public void SetInUI(bool isInUI)
    {
        if (this.IsInUI == isInUI) return;
        this.IsInUI = isInUI;
        this.gameObject.layer = IsInUI ? LayerMask.NameToLayer("UI") : LayerMask.NameToLayer("Actor");

        if (IsInUI)
        {
            _preUIPos = transform.position;
            transform.position = -50f * Vector3.forward;
        }
        else
        {
            transform.position = _preUIPos;
            transform.localScale = _initScale * Vector3.one;
        }

        if(IsPlayer())
        {
            MoveController.enabled = !IsInUI;
        }
    }

    private void FixedUpdate()
    {
        //HandleMoveFixedUpdate();
    }

    public int TakeDamage(int damage)
    {
        // Dodge graze
        if (MoveController.IsDodging) damage /= 2;

        // Last Chance (Players: If killing hit would do more than half health -> leave player at 1hp instead)
        if (IsPlayer())
        {
            if (damage >= Stats.Health && damage >= 0.4f * Stats.HealthMax.Value)
            {
                damage = Stats.Health - 1;
            }
        }

        // Do damage
        Stats.Health -= damage;
        return damage;
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.1f, dir, dist);
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
        if (IsInUI) targetS *= 5f;

        float lerpS = Mathf.Lerp(transform.localScale.x, targetS, 12f * Time.deltaTime);
        transform.localScale = lerpS * Vector3.one;
    }
    #endregion
}
