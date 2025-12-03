using System.Collections.Generic;
using UnityEngine;

public class SimpleNPCBrain : ActorBrain
{
    float _minTargetDist = 1f;
    float _maxTargetDist = 3f;
    float _lerpSpeed = 10f;
    Vector2 _moveDir;
    Vector2 _wanderVector = Vector2.up;
    float _wanderStrength = 0f;
    float _wanderSpeed = 1f;
    float _randPhase;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white.Alpha(0.25f);
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.color = Color.green.Alpha(0.25f);
        Gizmos.DrawWireSphere(transform.position, _maxTargetDist);
        Gizmos.DrawWireSphere(transform.position, _minTargetDist);
    }

    private void Start()
    {
        _randPhase = Random.Range(0f, 360f);
    }

    private void Update()
    {
        //FancyBrainUpdate1(Time.deltaTime);
        //BrainUpdate2(Time.deltaTime);
        BrainUpdate(Time.deltaTime);
    }

    void BrainUpdate(float deltaTime)
    {
        // Init
        Vector2 moveDir = Vector2.zero;
        _wanderVector = _wanderVector.Lerp(Random.insideUnitCircle, _wanderSpeed * deltaTime);
        //DebugDrawLine(_wanderVector, Color.yellow);
        var senses = _actor.Senses;
        float lerpMult = 1f;
        float moveMult = 1f;

        // Enemy
        if(senses.EnemyActors.Count > 0)
        {
            Vector2 enemyVector = Vector2.zero;
            foreach (Actor enemy in senses.EnemyActors)
            {
                if (enemy == null) continue;
                Vector2 towardsEnemy = enemy.transform.position - transform.position;
                float enemyDist = towardsEnemy.magnitude;
                towardsEnemy.Normalize();
                float desireMag = enemyDist.Remap(_minTargetDist, _maxTargetDist, -1f, 1f);
                desireMag = desireMag.Sign() * desireMag.Pow(800f).Abs();
                enemyVector += desireMag * towardsEnemy;
            }
            enemyVector /= senses.EnemyActors.Count;
            //enemyVector = enemyVector.Lerp(0.125f * _wanderVector, _wanderStrength);
            DebugDrawLine(enemyVector, Color.magenta);
            moveDir += enemyVector;
        }
        // No enemies
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
        Vector2 spatialVector = Vector2.zero;
        Vector2 openSpaceVector = _actor.Senses.WallVMap.ToVector() / 5f;
        if (openSpaceVector.magnitude > 1f) openSpaceVector.Normalize();
        openSpaceVector *= 0.6f;
        DebugDrawLine(openSpaceVector, Color.cyan);
        //moveDir += openSpaceVector;
        spatialVector += openSpaceVector;
        //
        // Avoid Walls
        Vector2Map wallMapClone = _actor.Senses.WallVMap.Clone();
        for (int i = 0; i < wallMapClone.Map.Count; i++)
        {
            float desireMag = wallMapClone.Map[i].magnitude.Remap(0.5f, 1f, 1f, 0f); // 0.2, 0.75
            wallMapClone.Map[i] = wallMapClone.Map[i].normalized * desireMag;
        }
        Vector2 avoidWallVector = 1f * -wallMapClone.ToVector().normalized; //0.4f * -wallMapClone.ToVector().normalized;
        DebugDrawLine(avoidWallVector, Color.blue);
        //moveDir += avoidWallVector;
        spatialVector += avoidWallVector;
        //
        DebugDrawLine(spatialVector, Color.Lerp(Color.blue, Color.cyan, 0.5f));
        moveDir += spatialVector;

        // Avoid allys
        if(_actor.Senses.AllyActors.Count > 0)
        {
            List<Vector2> allyDirections = new List<Vector2>();
            _actor.Senses.AllyActors.ForEach(x =>
            {
                if (x == null) return;
                var vector = x.transform.position - transform.position;
                float desire = vector.magnitude.Remap(2f, 8f, -1f, 0f);
                desire *= 0.5f;
                vector = desire * vector.normalized;
                allyDirections.Add(vector);
            });
            Vector2 averageAllyDir = allyDirections.AverageVectors();
            Vector2 avoidanceVector = averageAllyDir;
            DebugDrawLine(avoidanceVector, Color.yellow);
            moveDir += avoidanceVector;
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
            Vector2 skillAvoidanceVector = 0.25f * skillAvoidanceVectorList.AverageVectors(); //2.3
            DebugDrawLine(skillAvoidanceVector, Color.red);
            moveDir += skillAvoidanceVector;
        }

        // Lerp & Move
        _moveDir = _moveDir.Lerp(moveMult * moveDir.normalized, lerpMult * _lerpSpeed * deltaTime);
        _actor.Move(_moveDir);
        //DebugDrawLine(_moveDir, Color.green);
    }

    void BrainUpdate2(float deltaTime)
    {
        // Init
        Vector2 moveDir = Vector2.zero;

        // Wander Update
        _wanderVector = _wanderVector.Lerp(Random.insideUnitCircle.normalized, _wanderSpeed * Time.deltaTime).normalized;

        DebugDrawLine(_wanderVector, Color.gray);

        var enemies = _actor.Senses.EnemyActors;
        if(enemies.Count == 0)
        {
            // No enemy -> wander
            float distFromCenter = transform.position.magnitude;
            Vector2 towardsCenter = -transform.position.normalized;
            float t = distFromCenter.Remap(7f, 12f, 0f, 1f);
            Vector2 randomVector = _wanderVector.Lerp(towardsCenter, t);
            moveDir += randomVector;
        }
        else
        {
            // Sees enemy
            Vector2 targetDir = enemies[0].transform.position - transform.position;
            DebugDrawLine(targetDir, Color.magenta);
            targetDir = Vector2.Lerp(targetDir, _wanderVector, _wanderStrength);
            moveDir += targetDir;
        }

        _moveDir = _moveDir.Lerp(moveDir, _lerpSpeed * deltaTime);
        _actor.Move(_moveDir);
        DebugDrawLine(_moveDir, Color.green);
    }

    void FancyBrainUpdate1(float deltaTime)
    {
        // Init
        Vector2 moveDir = Vector2.zero;
        float lerpMult = 1f;

        // Enemy
        Vector2 enemyVector = Vector2.zero;
        foreach(Actor enemy in _actor.Senses.EnemyActors)
        {
            Vector2 towardsEnemy = enemy.transform.position - transform.position;
            float enemyDist = towardsEnemy.magnitude;
            towardsEnemy.Normalize();
            float desireMag = enemyDist.Remap(_minTargetDist, _maxTargetDist, -1f, 1f);
            enemyVector += desireMag * towardsEnemy;
        }
        enemyVector.Normalize();
        DebugDrawLine(enemyVector, Color.magenta);
        moveDir += enemyVector;

        // Scent

        // Ally

        // SPATIAL
        // Open Space
        Vector2 spatialVector = Vector2.zero;
        Vector2 openSpaceVector = _actor.Senses.WallVMap.ToVector() / 5f;
        if (openSpaceVector.magnitude > 1f) openSpaceVector.Normalize();
        openSpaceVector *= 0.6f;
        DebugDrawLine(openSpaceVector, Color.cyan);
        //moveDir += openSpaceVector;
        spatialVector += openSpaceVector;
        //
        // Avoid Walls
        Vector2Map wallMapClone = _actor.Senses.WallVMap.Clone();
        for(int i = 0; i < wallMapClone.Map.Count; i++)
        {
            float desireMag = wallMapClone.Map[i].magnitude.Remap(1f, 2f, 1f, 0f); // 0.2, 0.75
            wallMapClone.Map[i] = wallMapClone.Map[i].normalized * desireMag;
        }
        Vector2 avoidWallVector = 0.5f * -wallMapClone.ToVector().normalized; //0.4f * -wallMapClone.ToVector().normalized;
        DebugDrawLine(avoidWallVector, Color.blue);
        //moveDir += avoidWallVector;
        spatialVector += avoidWallVector;
        //
        DebugDrawLine(spatialVector, Color.Lerp(Color.blue, Color.cyan, 0.5f));
        moveDir += spatialVector;

        // Avoid allys
        List<Vector2> allyDirections = new List<Vector2>();
        _actor.Senses.AllyActors.ForEach(x =>
        {
            allyDirections.Add(x.transform.position - transform.position);
        });
        Vector2 averageAllyDir = allyDirections.AverageVectors();
        Vector2 allyAoidanceVector = -averageAllyDir;
        allyAoidanceVector = allyAoidanceVector.normalized * 0.4f;
        DebugDrawLine(allyAoidanceVector, Color.yellow);
        moveDir += allyAoidanceVector;

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

                DebugDrawLine(aimVector * 10f, Color.yellow);

                Vector2 evadeVector = -toSkillVector.normalized;
                DebugDrawLine(evadeVector, Color.green.Lerp(Color.white, 0.5f));
                evadeVector -= aimVector.normalized * 0.95f;
                evadeVector = evadeVector.normalized;
                DebugDrawLine(evadeVector, Color.green.Lerp(Color.blue, 0.0f));
                //if (evadeVector.magnitude > 1f) evadeVector = evadeVector.normalized;

                evadeVector = (evadeVector + _actor.Body.linearVelocity.normalized * 0.15f).normalized;

                float avoidancePercent = Mathf.Pow(dist.Remap(0.5f, 7f, 1f, 0f), 2f);
                lerpMult += avoidancePercent.Remap(0f, 1f, 0f, 0.01f);
                Vector2 avoidanceVector = evadeVector.normalized * avoidancePercent;

                skillAvoidanceVectorList.Add(avoidanceVector);
            });
            Vector2 skillAvoidanceVector = 1.5f * skillAvoidanceVectorList.AverageVectors(); //2.3
            DebugDrawLine(skillAvoidanceVector, Color.red);
            moveDir += skillAvoidanceVector;
        }

        // Lerp & Move
        _moveDir = _moveDir.Lerp(moveDir.normalized, lerpMult * _lerpSpeed * deltaTime);
        _actor.Move(_moveDir);
        DebugDrawLine(_moveDir, Color.green);
    }

    void DebugDrawLine(Vector3 vector, Color c)
    {
        Debug.DrawLine(transform.position, transform.position + vector, c);
    }
}
