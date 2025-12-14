using System.Collections.Generic;
using UnityEngine;

public class SimpleNPCBrain_Old : ActorBrain
{
    [SerializeField] float _preferedDistScale = 1f;
    [SerializeReference] float _openSpaceDesire = 0.6f;

    float _minDistBase = 1f;
    float _maxDistBase = 2.5f;
    float _minTargetDist => _minDistBase * _preferedDistScale; // 0.5
    float _maxTargetDist => _maxDistBase * _preferedDistScale; // 2

    [SerializeField] float _evasion = 0f; // 0.5f, 0.25f
    [SerializeField] float _wanderStrength = 0.5f; // 0.25f
    float _wanderSpeed = 1f; // 1
    [SerializeField] float _dodgeMult = 0f; // 1f
    [SerializeField] float _rushMult = 0f;

    float _lerpSpeed = 12f;

    Vector2 _moveDir;
    Vector2 _wanderVector = Vector2.up;
    
    float _randPhase;

    float _dodgeTheta;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white.Alpha(0.25f);
        Gizmos.DrawWireSphere(transform.position, (_minDistBase + _minTargetDist) / 2f);
    }

    private void Start()
    {
        _randPhase = Random.Range(0f, 360f);
    }

    private void Update()
    {
        //FancyBrainUpdate1(Time.deltaTime);
        //BrainUpdate2(Time.deltaTime);
        //BrainUpdate3(Time.deltaTime);

        //OldBrainUpdate(Time.deltaTime);

        PrototypeBrainUpdate(Time.deltaTime);
    }

    void PrototypeBrainUpdate(float deltaTime)
    {
        _wanderSpeed = 10.5f;
        _wanderStrength = 0.5f;

        _wanderVector = Vector2.Lerp(_wanderVector.normalized, Random.insideUnitCircle.normalized, _wanderSpeed * Time.deltaTime).normalized;

        var senses = _actor.Senses;
        if (senses.EnemyActors.Count == 0)
        {
            if (senses.EnemyScentDrop.Count == 0)
            {
                // Has no scent or sight
                float distFromCenter = transform.position.magnitude;
                Vector2 towardsCenter = -transform.position.normalized;

                float t = distFromCenter.Remap(7f, 12f, 0f, 1f);
                Vector2 randomMoveVector = Vector2.Lerp(_wanderVector, towardsCenter, t);

                _actor.MoveController.Ctrl_Move(randomMoveVector.normalized);
                Debug.DrawLine(transform.position, transform.position + (Vector3)_wanderVector * 5f, Color.gray);
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
            Vector2 moveDir = Vector2.Lerp(targetDir, _wanderVector, _wanderStrength);
            _actor.MoveController.Ctrl_Move(moveDir.normalized);
            Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir * 5f, Color.red);
        }
    }

    void BrainUpdate3(float deltaTime)
    {
        _wanderVector = Vector2.Lerp(_wanderVector.normalized, Random.insideUnitCircle.normalized, _wanderSpeed * Time.deltaTime).normalized;
        var senses = _actor.Senses;

        if (senses.EnemyActors.Count == 0)
        {
            if (senses.EnemyScentDrop.Count == 0)
            {
                // Has no scent or sight
                float distFromCenter = transform.position.magnitude;
                Vector2 towardsCenter = -transform.position.normalized;

                float t = distFromCenter.Remap(7f, 12f, 0f, 1f);
                Vector2 randomMoveVector = Vector2.Lerp(_wanderVector, towardsCenter, t);

                _actor.MoveController.Ctrl_Move(randomMoveVector);
                Debug.DrawLine(transform.position, transform.position + (Vector3)_wanderVector * 5f, Color.gray);
                return;
            }
            else
            {
                // Has scent but no sight
                Vector2 targetDir = senses.EnemyScentDrop[0].transform.position - transform.position;
                Vector2 moveDir = targetDir;// Vector2.Lerp(targetDir, _wanderVector, WanderStrength);
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
            Vector2 moveDir = Vector2.Lerp(targetDir, _wanderVector, _wanderStrength);
            _actor.MoveController.Ctrl_Move(moveDir.normalized);
            Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir * 5f, Color.red);
        }
    }

    void OldBrainUpdate(float deltaTime)
    {
        _dodgeTheta += 0.5f * Random.Range(0.75f, 1.25f) * Time.deltaTime;

        // Init
        Vector2 moveDir = Vector2.zero;
        _wanderVector = _wanderVector.Lerp(Random.insideUnitCircle, _wanderSpeed * deltaTime);
        DebugDrawLine(_wanderVector, Color.cyan);
        var senses = _actor.Senses;
        float lerpMult = 1f;
        float moveMult = 1f;
        float sprintMult = 1.01f;

        // Enemy
        if(senses.EnemyActors.Count > 0)
        {
            Vector2 enemyVector = Vector2.zero;
            float minEnemyDist = float.MaxValue;
            foreach (Actor enemy in senses.EnemyActors)
            {
                if (enemy == null) continue;
                Vector2 enemyPoint = (Vector2)enemy.transform.position + 0.25f * enemy.Body.linearVelocity;
                Vector2 towardsEnemy = enemyPoint - (Vector2)transform.position;
                float enemyDist = towardsEnemy.magnitude;
                if (enemyDist < minEnemyDist) minEnemyDist = enemyDist;
                towardsEnemy.Normalize();
                float desireMag = enemyDist.Remap(_minTargetDist, _maxTargetDist, -1f, 1f);
                desireMag = desireMag.Sign() * desireMag.Pow(8f).Abs();
                enemyVector += desireMag * towardsEnemy;
            }
            enemyVector /= senses.EnemyActors.Count;
            enemyVector = enemyVector.Lerp(_wanderVector.normalized, _wanderStrength);
            DebugDrawLine(enemyVector, Color.magenta);
            moveDir += enemyVector;
            float sprintPercent = minEnemyDist.Remap(_maxTargetDist, _maxTargetDist * 2f, 0f, 1f);
            _dodgeTheta += sprintPercent * Time.deltaTime;
            moveMult *= sprintPercent.RemapPercent(1f, sprintMult);
        }
        // Scent
        else if (senses.EnemyActors.Count == 0 && senses.EnemyScentDrop.Count > 0)
        {
            Vector2 scentPos = senses.EnemyScentDrop[0].transform.position;
            Vector2 scentDir = scentPos - (Vector2)transform.position;
            scentDir.Normalize();
            DebugDrawLine(scentDir, Color.red);
            moveDir += scentDir;
            moveMult *= sprintMult;
        }
        // No enemies or scents
        else
        {
            // Wander logic
            float distFromCenter = transform.position.magnitude;
            Vector2 towardsCenter = -transform.position.normalized;
            float t = distFromCenter.Remap(7f, 12f, 0f, 0.1f);
            Vector2 wanderDir = _wanderVector;//.Lerp(towardsCenter, t);
            DebugDrawLine(wanderDir, Color.white);
            moveDir += 0.5f * wanderDir;
            lerpMult *= 0.5f;
            moveMult = Mathf.Sin(0.25f * Mathf.PI * Time.time + _randPhase).Remap(-1f, 1f, 0f, 1f);
        }

        // SPATIAL
        // Open Space
        Vector2 openSpaceVector = _actor.Senses.WallVMap.ToVector() / 5f;
        if (openSpaceVector.magnitude > 1f) openSpaceVector.Normalize();
        openSpaceVector *= _openSpaceDesire;
        DebugDrawLine(openSpaceVector, Color.cyan);
        moveDir += openSpaceVector;
        //
        // Avoid Walls
        /*
        Vector2Map wallMapClone = _actor.Senses.WallVMap.Clone();
        float minWallDist = float.MaxValue;
        for (int i = 0; i < wallMapClone.Map.Count; i++)
        {
            float wallDist = wallMapClone.Map[i].magnitude;
            if (wallDist < minWallDist) minWallDist = wallDist;
            float desireMag = wallDist.Remap(0.25f, 0.5f, 1f, 0f); // 0.2, 0.75
            //desireMag = desireMag.Sign() * desireMag.Pow(2f).Abs();
            wallMapClone.Map[i] = wallMapClone.Map[i].normalized * desireMag;
        }
        Vector2 avoidWallVector = 0.25f * minWallDist.Remap(0.25f, 0.5f, 1f, 0f).Pow(2f) * -wallMapClone.ToVector().normalized; //0.4f * -wallMapClone.ToVector().normalized;
        DebugDrawLine(avoidWallVector, Color.blue);
        //moveDir += avoidWallVector;
        spatialVector += avoidWallVector;
        //
        DebugDrawLine(spatialVector, Color.Lerp(Color.blue, Color.cyan, 0.5f));
        moveDir += spatialVector;
        */
        var wallMap = _actor.Senses.WallVMap;
        float minWallDist = float.MaxValue;
        wallMap.Map.ForEach(dir =>
        {
            var dist = dir.magnitude;
            if (dist < minWallDist) minWallDist = dist;
        });
        float wallMult = minWallDist.Remap(0.25f, 0.75f, 1f, 0f); 
        Vector2 avoidWallVector = wallMap.ToVector() * 0.125f * wallMult;
        DebugDrawLine(avoidWallVector, Color.blue);
        moveDir += avoidWallVector;
        

        // Avoid allys
        if (_actor.Senses.AllyActors.Count > 0)
        {
            List<Vector2> allyDirections = new List<Vector2>();
            float minAllyDist = float.MaxValue;
            _actor.Senses.AllyActors.ForEach(x =>
            {
                if (x == null) return;
                var vector = x.transform.position - transform.position;
                float dist = vector.magnitude;
                if (dist < minAllyDist) minAllyDist = dist;
                float desire = vector.magnitude.Remap(_minTargetDist, _maxTargetDist, -1f, 0f);
                desire *= 0.5f;
                vector = desire * vector.normalized;
                allyDirections.Add(vector);
            });
            Vector2 averageAllyDir = allyDirections.AverageVectors();
            DebugDrawLine(averageAllyDir, Color.yellow);
            moveDir += averageAllyDir;
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
                lerpMult += avoidancePercent.Remap(0f, 1f, 0f, 0.01f);
                Vector2 avoidanceVector = evadeVector.normalized * avoidancePercent;

                skillAvoidanceVectorList.Add(avoidanceVector);
            });
            var avoidanceVector = skillAvoidanceVectorList.AverageVectors();

            // Avoidance Dodge
            _dodgeTheta += avoidanceVector.magnitude * Time.deltaTime;
            if (avoidanceVector.magnitude >= 0.7f && _dodgeMult * Mathf.Sin(_dodgeTheta) > 0.75f) _actor.MoveController.Ctrl_Dodge(avoidanceVector.normalized);

            Vector2 skillAvoidanceVector = _evasion * avoidanceVector; //2.3f
            DebugDrawLine(skillAvoidanceVector, Color.red);
            moveDir += skillAvoidanceVector;
        }

        // Lerp & Move
        moveDir = moveDir.magnitude <= 0.01f ? Vector2.zero : moveDir.normalized;
        _moveDir = _moveDir.Lerp(moveDir, lerpMult * _lerpSpeed * deltaTime);
        Vector2 moveVector = moveMult * _moveDir;

        // Move Dodge
        if (moveMult > 1)
        {
            if (_rushMult * Mathf.Sin(_dodgeTheta) > 0.9f) _actor.MoveController.Ctrl_Dodge(moveVector.normalized);
        }

        _actor.MoveController.Ctrl_Move(moveVector);
        DebugDrawLine(moveVector, Color.green);
    }

    void DebugDrawLine(Vector3 vector, Color c)
    {
        Debug.DrawLine(transform.position, transform.position + vector, c);
    }
}
