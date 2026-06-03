using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleNPCBrain : ActorBrain
{
    public string State;

    public float DifficultyMult = 1f;

    [Header("Dist")]
    [SerializeField] float _passiveDistMult = 1f;
    [SerializeField] float _attackDistMult = 0.5f;
    float _minDistBase = 1f;
    float _maxDistBase = 2.5f;
    float _minDistPref => _minDistBase * _distMult; // 0.5,  1f
    float _maxDistPref => _maxDistBase * _distMult; // 2, 2.5
    float _distMult => _skillPassiveDistMult * _skillAttackChase.RemapPercent(_passiveDistMult, _attackDistMult, false);// _isAttacking ? _aiAttackMag * _attackDistMult : _passiveDistMult;
    float _skillAttackChase => _skillSystem?.GetAI_AttackChase() ?? 0f;
    float _skillPassiveDistMult => _skillSystem?.GetAI_PassiveDistMult() ?? 1f;
    bool _isAttacking => _skillSystem?.GetAI_IsAttacking() ?? false;
    ActorSkillSystem _skillSystem => _actor?.SkillSystem;

    [Header("Walls")]
    [SerializeField] float _openSpaceDesire = 0f;

    [Header("Rand")]
    [SerializeField] float _noiseMag = 0.5f;
    [SerializeField] float _noiseFreq = 1f;
    [SerializeField] float _noiseFloor = 0f;
    float _idleRestTimer = 0f;
    float _idleMoveTimer = 0f;

    [Header("Evade")]
    [SerializeField] float _evasion = 0f;
    [SerializeField] float _dodgeEvade = 0f;
    float _dodgeEvadeCharge;
    [SerializeField] float _dodgeSprint = 0f;
    float _dodgeSprintCharge;

    float debugAiAttackMag;
    float debugPassiveDistMult;

    float _smellTick = 0f;

    // State
    public float NoticeMag;
    public float AttackMag;
    public float TrackingMag;
    float _lastNoticeMag;
    float _lastTrackingMag;

    // Events
    public Action OnNotice;
    public Action OnLostTrail;


    private void OnDrawGizmosSelected()
    {
        /*
        Gizmos.color = Color.yellow.Alpha(_isAttacking? 0.1f : 0.25f);
        Gizmos.DrawWireSphere(transform.position, + _passiveDistMult * (_minDistBase + _maxDistBase) / 2f);
        Gizmos.color = Color.red.Alpha(!_isAttacking? 0.1f : 0.25f);
        Gizmos.DrawWireSphere(transform.position, + _attackDistMult * (_minDistBase + _maxDistBase) / 2f);
        */
        Gizmos.color = Color.red.Lerp(Color.white, 0.5f).Alpha(0.5f);
        Gizmos.DrawWireSphere(transform.position, _minDistPref);
        Gizmos.color = Color.blue.Lerp(Color.white, 0.5f).Alpha(0.5f);
        Gizmos.DrawWireSphere(transform.position, _maxDistPref);
    }

    private void Start()
    {
        _actor.OnTakeDamage += () =>
        {
            if(NoticeMag < 1f) NoticeMag += 1f;
        };
    }

    private void OnEnable()
    {
        _dodgeEvadeCharge = Random.Range(0f, 1f);
        _dodgeSprintCharge = Random.Range(0f, 1f);
        _seed = Random.Range(0f, 10f);

        NoticeMag = -0.5f;
    }

    private void Update()
    {
        debugAiAttackMag = _actor?.SkillSystem?.GetAI_AttackChase() ?? 0.5f;
        debugPassiveDistMult = _actor?.SkillSystem?.GetAI_PassiveDistMult() ?? 1f;
        //PrototypeBrainUpdate(Time.deltaTime);
        BrainUpdate(Time.deltaTime);

        // Noise Test
        //Debug.DrawLine(transform.position, transform.position + (Vector3)GetPerlinVector(), Color.cyan.Alpha(0.75f));
        //_actor.MoveController.Ctrl_Move(GetPerlinVector());

        if(_lastNoticeMag < 1f && NoticeMag >= 1f)
        {
            //Debug.Log("Notice");
            OnNotice?.Invoke();
        }

        if(_lastNoticeMag >= 1f && NoticeMag < 1f && _lastTrackingMag > 0f && TrackingMag <= 0f)
        {
            //Debug.Log("Lost Trail");
            OnLostTrail?.Invoke();
        }
    }

    private void LateUpdate()
    {
        _lastNoticeMag = NoticeMag;
        _lastTrackingMag = TrackingMag;
    }

    float _seed;
    Vector2 GetPerlinVector()
    {
        Random.InitState(_seed.GetHashCode());
        var noiseVector = _noiseMag * new Vector2(Mathf.PerlinNoise(_noiseFreq * Time.time + Random.Range(0f, 1f), Random.Range(0f, 1f)).RemapPercent(-1f, 1f), Mathf.PerlinNoise(Random.Range(0f, 1f), _noiseFreq * Time.time + Random.Range(0f, 1f)).RemapPercent(-1f, 1f));
        noiseVector = noiseVector.normalized * noiseVector.magnitude.Remap(_noiseFloor, 1f, 0f, 1f);
        return noiseVector;
    }


    Vector2 _noiseVector;
    void BrainUpdate(float deltaTime)
    {
        // INIT
        var senses = _actor.Senses;
        Vector2 moveDir = Vector2.zero;
        float sprintMult = 1f;

        // UPDATES
        //
        // Wander
        _noiseVector = GetPerlinVector();// Vector2.Lerp(_noiseVector.normalized, Random.insideUnitCircle.normalized, 10.5f * deltaTime).normalized;
        DebugDrawLine(_noiseVector, Color.white.Alpha(0.5f));
        //
        // Dodge
        _dodgeEvadeCharge += DifficultyMult * _dodgeEvade * deltaTime;
        if (_dodgeEvadeCharge > 1f) _dodgeEvadeCharge = 1f;
        _dodgeSprintCharge += DifficultyMult * _dodgeSprint * deltaTime;
        if (_dodgeSprintCharge > 1f) _dodgeSprintCharge = 1f;

        // CHASE
        //
        // Enemy
        if (NoticeMag < 1f && senses.EnemyActors.Count > 0)
        {
            float distMult = 1f;
            senses.EnemyActors.ForEach(enemyActor =>
            {
                float dist = Vector2.Distance(enemyActor.transform.position, transform.position);
                distMult *= dist.Remap(1f, 5f, 2f, 1f);
            });
            float countMult = senses.EnemyActors.Count;
            float noticeMult = countMult * distMult;
            NoticeMag += noticeMult * 0.75f * Time.deltaTime;
        }
        else
        {
            if (senses.EnemyActors.Count > 0)
            {
                // Brian params
                AttackMag = _isAttacking ? 1f : 0f;
                TrackingMag = 0f;
                _smellTick = 0f;
                State = _isAttacking ? "Attacking Enemy" : "Chasing Enemy";
                _idleRestTimer = 0f;

                Vector2 desireVector = Vector2.zero;
                float averageDistance = 0f;

                foreach (Actor enemy in senses.EnemyActors)
                {
                    Vector2 towardsEnemy = enemy.transform.position - transform.position;
                    float desire = towardsEnemy.magnitude.Remap(_minDistPref, _maxDistPref, -1f, 1f);
                    //desire = desire.Sign() * desire.Pow(8f).Abs();
                    desireVector += desire * towardsEnemy.normalized;
                    averageDistance += towardsEnemy.magnitude;
                }
                desireVector /= senses.EnemyActors.Count;

                //DebugDrawLine(desireVector, Color.red.Lerp(Color.white, 0.5f));
                DebugDrawLine(desireVector, Color.red.Lerp(Color.white, 0.5f).Alpha(0.5f));

                // desire wander
                float mag = desireVector.magnitude;
                desireVector = mag * desireVector.Lerp(_noiseVector, _noiseMag * (_isAttacking ? 0.5f : 1f)).normalized;


                moveDir += desireVector;

                // enemy sprint
                sprintMult += averageDistance.Remap(_maxDistPref, _maxDistPref * 2f, 0f, 0.05f);
            }
            //
            // Scent
            else if (senses.EnemyScentDrop.Count > 0 && senses.EnemyScentDrop[0] != null && _smellTick < 2.5f)
            {
                // Brian params
                AttackMag = 0f;
                TrackingMag = 1f;

                _smellTick += Random.Range(0.75f, 1.25f) * Time.deltaTime;
                State = "Tracking Scent";
                sprintMult += 0.05f;
                Vector2 targetDir = senses.EnemyScentDrop[0].transform.position - transform.position;
                moveDir += targetDir.normalized;
                Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir * 5f, Color.green);
            }
            else
            {
                // Brian params
                AttackMag = 0f;
                if(NoticeMag > 0f)
                {
                    if (NoticeMag > 1f) NoticeMag = 1f;
                    NoticeMag -= 0.025f * Time.deltaTime;
                }
                TrackingMag = 0f;
            }
        }
        //
        // Idle
        if (NoticeMag < 1f) 
        {
            // Brian params
            AttackMag = 0f;
            TrackingMag = 0f;

            State = "Idle";

            if (_idleMoveTimer > 0f)
            {
                _idleMoveTimer -= Time.deltaTime;
                float distFromCenter = transform.position.magnitude;
                Vector2 towardsCenter = -transform.position.normalized;

                float t = distFromCenter.Remap(7f, 12f, 0f, 1f);
                Vector2 randomMoveVector = Vector2.Lerp(_noiseVector, towardsCenter, t);

                moveDir += randomMoveVector.normalized;

                if(_idleMoveTimer <= 0f)
                {
                    float maxRestTime = NoticeMag.RemapPercent(10f, 0f);
                    _idleRestTimer += Random.Range(0f, maxRestTime);
                }
            }
            else
            {
                _idleRestTimer -= Time.deltaTime;
                if(_idleRestTimer <= 0f)
                {
                    float minMovetime = NoticeMag.RemapPercent(0f, 5f);
                    _idleMoveTimer += Random.Range(minMovetime, 10f);
                }
                _actor.MoveController.Ctrl_Move(Vector2.zero);
                return;
            }
        }
        //===

        // SPATIAL
        //
        // Open Space
        if (senses.EnemyActors.Count > 0)
        {
            Vector2 openSpaceVector = _actor.Senses.WallVMap.ToVector() / 5f;
            if (openSpaceVector.magnitude > 1f) openSpaceVector.Normalize();
            openSpaceVector *= _openSpaceDesire;
            DebugDrawLine(openSpaceVector, Color.blue.Lerp(Color.black, 0.5f));
            moveDir += openSpaceVector;
        }
        //
        // Avoid Wall
        var wallMap = _actor.Senses.WallVMap;
        float minWallDist = float.MaxValue;
        Vector2 wallVector = Vector2.zero;
        wallMap.Map.ForEach(dir =>
        {
            wallVector += dir.magnitude.Remap(0.4f, 0.8f, 1f, 0f) * dir.normalized;
            var dist = dir.magnitude;
            if (dist < minWallDist) minWallDist = dist;
        });
        wallVector /= wallMap.Map.Count;
        wallVector *= -1f;
        wallVector *= 10f;
        float wallMult = minWallDist.Remap(0.4f, 0.8f, 1f, 0f);
        Vector2 avoidWallVector = wallMap.ToVector() * 0.25f * wallMult; // 0.125f
        // wall
        //DebugDrawLine(avoidWallVector, Color.blue);
        //moveDir += wallVector;
        //===

        // Avoid Allys
        var allyList = _actor.Senses.AllyActors;
        if(allyList.Count > 0)
        { 
        }

        // Avoid Enemy Skills
        var enemySkills = _actor.Senses.EnemySkills;
        if (enemySkills.Count > 0)
        {
            List<Vector2> skillAvoidanceVectorList = new List<Vector2>();
            _actor.Senses.EnemySkills.ForEach(x =>
            {
                Vector2 toSkillVector = transform.VectorTowards2D(x.transform);
                float dist = toSkillVector.magnitude;

                Vector2 aimVector = x.transform.up;
                Rigidbody2D skillBody = x.GetComponent<Rigidbody2D>();
                if (skillBody != null && skillBody.linearVelocity.sqrMagnitude > 0.1f)
                {
                    aimVector = skillBody.linearVelocity.normalized;
                }

                //DebugDrawLine(aimVector * 10f, Color.yellow);

                Vector2 evadeVector = -toSkillVector.normalized;
                //DebugDrawLine(evadeVector, Color.green.Lerp(Color.white, 0.5f));
                evadeVector -= aimVector.normalized * 0.95f;
                evadeVector = evadeVector.normalized;
                //DebugDrawLine(evadeVector, Color.green.Lerp(Color.blue, 0.0f));
                //if (evadeVector.magnitude > 1f) evadeVector = evadeVector.normalized;

                evadeVector = (evadeVector + _actor.Body.linearVelocity.normalized * 0.15f).normalized;

                float avoidancePercent = Mathf.Pow(dist.Remap(0.5f, 7f, 1f, 0f), 2f);
                //lerpMult += avoidancePercent.Remap(0f, 1f, 0f, 0.01f);
                Vector2 avoidanceVector = evadeVector.normalized * avoidancePercent;

                skillAvoidanceVectorList.Add(avoidanceVector);
            });
            var avoidanceVector = skillAvoidanceVectorList.AverageVectors();

            // Avoidance Dodge
            if (_dodgeEvadeCharge >= 1f && avoidanceVector.magnitude >= 0.7f && _actor.MoveController.IsDodgeCooledDown)
            {
                _dodgeEvadeCharge = 0f;
                _actor.MoveController.Ctrl_Dodge(avoidanceVector.normalized);
            }

            // Apply Evasion
            Vector2 skillAvoidanceVector = DifficultyMult * _evasion * avoidanceVector; //2.3f
            DebugDrawLine(skillAvoidanceVector, Color.magenta);
            moveDir += skillAvoidanceVector;
        }

        // Sprint Dodge
        if (_dodgeSprintCharge >= 1f && sprintMult > 1 && _actor.MoveController.IsDodgeCooledDown)
        {
            _dodgeSprintCharge = 0f;
            _actor.MoveController.Ctrl_Dodge(moveDir.normalized);
        }

        // MOVE
        moveDir = sprintMult * moveDir.ClampMagnitude(1f);
        _actor.MoveController.Ctrl_Move(moveDir);
        Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir, Color.cyan);
        //===
    }

    void PrototypeBrainUpdate(float deltaTime)
    {
        float _wanderSpeed = 10.5f;
        float _wanderStrength = 0.5f;

        _noiseVector = Vector2.Lerp(_noiseVector.normalized, Random.insideUnitCircle.normalized, _wanderSpeed * Time.deltaTime).normalized;
        DebugDrawLine(_noiseVector, Color.white);

        var senses = _actor.Senses;
        if (senses.EnemyActors.Count == 0)
        {
            if (senses.EnemyScentDrop.Count == 0)
            {
                // Has no scent or sight
                float distFromCenter = transform.position.magnitude;
                Vector2 towardsCenter = -transform.position.normalized;

                float t = distFromCenter.Remap(7f, 12f, 0f, 1f);
                Vector2 randomMoveVector = Vector2.Lerp(_noiseVector, towardsCenter, t);

                _actor.MoveController.Ctrl_Move(randomMoveVector.normalized);
                Debug.DrawLine(transform.position, transform.position + (Vector3)_noiseVector * 5f, Color.gray);
                return;
            }
            else
            {
                // Has scent but no sight
                Vector2 targetDir = senses.EnemyScentDrop[0].transform.position - transform.position;
                Vector2 moveDir = targetDir;// Vector2.Lerp(targetDir, randomDir, WanderStrength);
                _actor.MoveController.Ctrl_Move(moveDir.normalized);
                Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir * 5f, Color.green);
                return;
            }
        }
        else
        {
            // Has sight
            GameObject targetPlayerObject = senses.EnemyActors[0].gameObject;
            Vector2 targetDir = targetPlayerObject.transform.position - transform.position;
            DebugDrawLine(targetDir, Color.red.Lerp(Color.white, 0.5f));
            Vector2 moveDir = Vector2.Lerp(targetDir, _noiseVector, _wanderStrength);
            _actor.MoveController.Ctrl_Move(moveDir.normalized);
            Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir, Color.red);
        }
    }

    void DebugDrawLine(Vector3 vector, Color c)
    {
        Debug.DrawLine(transform.position, transform.position + vector, c);
    }
}
