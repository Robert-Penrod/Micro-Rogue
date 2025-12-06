using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Actor))]
public class ActorSenses : MonoBehaviour
{
    float _wallSenseDist = 12f;
    float _actorSenseDist = 25f;
    float _skillSenseDist = 25f;

    public List<Actor> EnemyActors;
    public List<SkillInstance> EnemySkills;
    public List<ScentDrop> EnemyScentDrop;
    public List<Actor> AllyActors;
    public List<SkillInstance> AllySkills;
    public Vector2Map WallVMap = new(14);

    Actor _actor;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
    }

    private void OnDrawGizmosSelected()
    {
        // Walls
        //WallVMap.GizmoDraw(transform.position);

        // Actors
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        EnemyActors.ForEach(x =>
        {
            if (x == null) return;
            //Gizmos.DrawLine(transform.position, x.transform.position);
        });

        // Scents
        if (EnemyActors.Count == 0)
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
            for (int i = 0; i < 1 && i < EnemyScentDrop.Count; i++)
            {
                var drop = EnemyScentDrop[i];
                if (drop == null) continue;
                //Gizmos.DrawLine(transform.position, drop.transform.position);
            }
        }

        // Allys
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        AllyActors.ForEach(x =>
        {
            if (x == null) return;
            //Gizmos.DrawLine(transform.position, x.transform.position);
        });
    }

    TickTimer _wallScanTimer = new(0.1f);
    TickTimer _scentScanTimer = new(0.1f);
    TickTimer _actorScanTimer = new(0.25f);
    TickTimer _skillScanTimer = new(0.25f);

    private void Start()
    {
        _wallScanTimer.SetPercent(Random.Range(0f, 1f));
        _actorScanTimer.SetPercent(Random.Range(0f, 1f));
        _skillScanTimer.SetPercent(Random.Range(0f, 1f));
        _scentScanTimer.SetPercent(Random.Range(0f, 1f));
    }
    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        _wallScanTimer.Tick(deltaTime);
        _actorScanTimer.Tick(deltaTime);
        _skillScanTimer.Tick(deltaTime);
        _scentScanTimer.Tick(deltaTime);

        if(_wallScanTimer.IsDone())
        {
            _wallScanTimer.ResetByMaxTime();
            ScanForWalls();
        }

        if(_actorScanTimer.IsDone())
        {
            _actorScanTimer.ResetByMaxTime();
            ScanForActors();
        }

        if(_scentScanTimer.IsDone())
        {
            _scentScanTimer.ResetByMaxTime();
            ScanForScents();
        }

        if (_skillScanTimer.IsDone())
        {
            _skillScanTimer.ResetByMaxTime();
            ScanForSkills();
        }
    }

    void ScanForWalls()
    {
        WallVMap.Clear();
        for(int i = 0; i < WallVMap.Count; i++)
        {
            Vector2 dir = WallVMap.GetDir(i);
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.1f, dir, _wallSenseDist, LayerMask.GetMask("Default"));
            //RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + (Vector3)dir.normalized * _wallSenseDist);
            float dist = hit ? hit.distance : _wallSenseDist;
            WallVMap.Map[i] = dir * dist;
        }
    }

    void ScanForActors()
    {
        EnemyActors.Clear();
        AllyActors.Clear();
        List<Actor> actorList = Utils.ComponentScan<Actor>(transform.position, _actorSenseDist, true, LayerMask.GetMask("Actor"));
        actorList.ForEach(x =>
        {
            if (x == _actor) return;
            if (_actor.IsEnemyOf(x))
            {
                if (!_actor.HasLineOfSightOf(x)) return;
                if (!EnemyActors.Contains(x))EnemyActors.Add(x);
            }
            else
            {
                if (!AllyActors.Contains(x)) AllyActors.Add(x);
            }
        });
    }

    void ScanForScents()
    {
        EnemyScentDrop.Clear();
        EnemyScentDrop = Utils.ComponentScan<ScentDrop>(transform.position, _actorSenseDist, false, LayerMask.GetMask("Scent"));
        EnemyScentDrop.RemoveAll(scentDrop => !_actor.IsEnemyOf(scentDrop.ScentSystem.Actor) || !_actor.HasLineOfSightOf(scentDrop.transform.position));
        EnemyScentDrop.Sort((x, y) =>
        {
            return x.Age < y.Age? -1 : 1;
        });
    }

    void ScanForSkills()
    {
        EnemySkills.Clear();
        AllySkills.Clear();
        List<SkillInstance> skillInstanceList = Utils.ComponentScan<SkillInstance>(transform.position, _skillSenseDist, false, LayerMask.GetMask("Skill"));
        SkillInstance[] skillInstanceArray = skillInstanceList.ToArray();
        Array.Sort(skillInstanceArray, (x, y) =>
        {
            return (int)Mathf.Sign(transform.DistanceFrom(x.transform) - transform.DistanceFrom(y.transform));
        });

        skillInstanceList.ForEach(x =>
        {
            if (x.Skill.Actor.IsEnemyOf(_actor))
            {
                EnemySkills.Add(x);
            }
            else
            {
                AllySkills.Add(x);
            }
        });
    }
}
