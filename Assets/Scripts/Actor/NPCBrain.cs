using UnityEngine;

public class NPCBrain : ActorBrain
{
    [Header("State")]
    public string State;

    [Header("Params")]
    float _targetMinDist = 1f;
    float _targetMaxDist = 2.5f;
    [SerializeField] float _targetPassiveDistMult = 1f;
    [SerializeField] float _targetAttackDistMult = 0.75f;
    float _targetDistMult => _aiAttackMag > 0f ? _aiAttackMag * _targetAttackDistMult : _targetPassiveDistMult;
    bool _isAttacking => _aiAttackMag != 0f;

    float _allyAvoidDist = 1f;
    float _allyPursueDist = 10f;
    float _wanderMag = 0.5f;
    float _wanderFreq = 1f;
    float _evasion = 0f;

    Vector2 _wanderVector;
    Vector2 _lerpWanderVector;

    float _aiAttackMag => _actor?.SkillSystem?.GetAI_AttackChase() ?? 0.5f;

    private void Update()
    {
        BrainUpdate();
    }

    void BrainUpdate()
    {
        var senses = _actor.Senses;
        Vector2 moveDir = Vector2.zero;

        // CHASE
        //
        // Enemy
        if(senses.EnemyActors.Count > 0)
        {
            State = _isAttacking ? "Attacking Enemy" : "Chasing Enemy";
            
            Vector2 targetPos = _actor.Senses.EnemyActors[0].transform.position;
            Vector2 towardsTarget = targetPos - (Vector2)transform.position;
            float dist = towardsTarget.magnitude;
            towardsTarget.Normalize();

            float desire = dist.Remap(_targetMinDist * _targetDistMult, _targetMaxDist * _targetDistMult, -1f, 1f);
            Vector2 enemyMoveInfluence = desire * towardsTarget.normalized;

            moveDir += enemyMoveInfluence;
            Debug.DrawLine(transform.position, transform.position + (Vector3)enemyMoveInfluence, Color.red);
        }
        // Scent
        else if(senses.EnemyScentDrop.Count > 0 && senses.EnemyScentDrop[0] != null)
        {
            State = "Tracking Scent";
            Vector2 targetDir = senses.EnemyScentDrop[0].transform.position - transform.position;
            moveDir += targetDir.normalized;
            Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir, Color.red.Lerp(Color.white, 0.5f));
        }
        //
        // Idle
        else
        {
            State = "Idle";
        }

        // Move
        moveDir = moveDir.ClampMagnitude(1f);
        _actor.MoveController.Ctrl_Move(moveDir);
        Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir, Color.white);
    }
}
