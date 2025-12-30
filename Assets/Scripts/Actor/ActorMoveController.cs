using System;
using UnityEngine;

public class ActorMoveController : MonoBehaviour
{
    // Move
    public Vector2 MoveDir { get; private set; }

    // Dodge
    public bool IsDodging => _dodgeTimer > 0f;
    float _dodgeTimer;
    float DodgeCooldownTime => _actor.Stats.DodgeCooldown.Value;
    float _dodgeCooldownTick;
    public bool IsDodgeCooledDown => DodgeCooldownPercent >= 1f;
    public float DodgeCooldownPercent => DodgeCooldownTime > 0 ? _dodgeCooldownTick / DodgeCooldownTime : 0;

    // Reference
    Actor _actor;
    Rigidbody2D _body => _actor.Body;

    // Events
    public Action OnDodge;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
        _dodgeCooldownTick = DodgeCooldownTime;
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
        if (_dodgeCooldownTick < DodgeCooldownTime || MoveDir.sqrMagnitude < 0.01f) return false;

        // Dodge data
        _dodgeTimer = dodgeTimeMult * 0.25f;// * 0.325f;// * Constants.SkillStats.Duration.Melee;// * 0.325f;
        _dodgeCooldownTick = 0f;
        MoveDir = 2f * dodgeVector.normalized;

        // Camera Shake
        CamShaker.I.Shake(0.2f, 0.3f);

        // Skill speed dampen
        // todo

        // End
        OnDodge?.Invoke();
        return true;
    }

    private void FixedUpdate()
    {
        // Dodge
        if (_dodgeCooldownTick < DodgeCooldownTime) _dodgeCooldownTick += Time.fixedDeltaTime;
        if(IsDodging) _dodgeTimer -= Time.fixedDeltaTime;

        // Move
        float baseMoveSpeed = Constants.ActorStats.MoveSpeed.Default;
        _body.AddForce(baseMoveSpeed * MoveDir * _body.linearDamping);
    }
}
