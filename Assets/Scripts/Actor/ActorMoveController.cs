using System;
using UnityEngine;

public class ActorMoveController : MonoBehaviour
{
    public enum MoveTypeEnum { Walk = 0, Hop = 10}
    public MoveTypeEnum MoveType;

    // Move
    public Vector2 MoveDir { get; private set; }

    // Hop
    float _hopTick;

    // Dodge
    [SerializeField] AudioClip _dodgeSound;
    public bool IsDodging => _dodgeTimer > 0f;
    float _dodgeTimer;
    public bool IsDodgeCooledDown => DodgeCooldownPercent >= 1f;
    public float DodgeCooldownPercent;

    // Reference
    Actor _actor;
    Rigidbody2D _body => _actor.Body;

    float _initDrag;

    // Events
    public Action OnDodge;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
        DodgeCooldownPercent = 1f;

        _initDrag = _body.linearDamping;
        MoveDir = Vector2.zero;
    }

    public void Ctrl_Move(Vector2 moveDir)
    {
        if (IsDodging) return;

        if(moveDir.sqrMagnitude < 0.01f)
        {
            MoveDir = Vector2.zero;
        }
        MoveDir = moveDir;
    }

    public bool Ctrl_Dodge(Vector2 dodgeVector, float dodgeTimeMult = 1f)
    {
        // Dodge not cooled down || no move input
        if (DodgeCooldownPercent < 1f || MoveDir.sqrMagnitude < 0.01f) return false;

        // Dodge data
        _dodgeTimer = dodgeTimeMult * 0.325f;// * 0.325f;// * Constants.SkillStats.Duration.Melee;// * 0.325f;
        DodgeCooldownPercent = 0f;
        MoveDir = 2f * dodgeVector.normalized;

        // Camera Shake
        CamShaker.I.Shake(0.2f, 0.3f);

        // Skill speed dampen
        // todo

        AudioSpawner.PlayAudioWithRandPitch(_dodgeSound, 0.2f, 1f, 1f);

        // End
        OnDodge?.Invoke();
        return true;
    }

    private void FixedUpdate()
    {
        float hopFactor = 1f;

        // Dodge
        if (DodgeCooldownPercent < 1f) DodgeCooldownPercent += _actor.Stats.DodgeRate.Value * Time.fixedDeltaTime;
        if(IsDodging) _dodgeTimer -= Time.fixedDeltaTime;

        // Move
        var moveSpeed = _actor.Stats.MoveSpeed.Value;
        if (MoveType == MoveTypeEnum.Walk || IsDodging)
        {
            _body.AddDampForce(moveSpeed * MoveDir * _body.mass);
        }        
        else if(MoveType == MoveTypeEnum.Hop)
        {
            if(_hopTick < 1f) _hopTick += 0.666f * moveSpeed * Time.deltaTime / hopFactor;
            if(_hopTick >= 1f && MoveDir.magnitude > 0.01f)
            {
                _hopTick -= 1f;
                _body.AddDecayForce(hopFactor * 0.25f * moveSpeed * MoveDir.normalized);
                _body.AddDampForce(hopFactor * 1f * moveSpeed * MoveDir.normalized);
            }
        }
    }
}
