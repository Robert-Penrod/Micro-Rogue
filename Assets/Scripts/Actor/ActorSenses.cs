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
    public List<Actor> AllyActors;
    public List<SkillInstance> AllySkills;
    public Vector2Map WallVMap = new(16);

    Actor _actor;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
    }

    private void OnDrawGizmosSelected()
    {
        // Walls
        WallVMap.GizmoDraw(transform.position);

        // Actors
        Gizmos.color = new Color(1f, 0f, 0f, 0.125f);
        EnemyActors.ForEach(x =>
        {
            if (x == null) return;
            Gizmos.DrawLine(transform.position, x.transform.position);
        });

        // Allys
        Gizmos.color = new Color(0f, 1f, 0f, 0.125f);
        AllyActors.ForEach(x =>
        {
            if (x == null) return;
            Gizmos.DrawLine(transform.position, x.transform.position);
        });
    }

    private void Start()
    {
        InvokeRepeating("ScanForWalls", Random.Range(0f, 0.5f), 0.1f);
        InvokeRepeating("ScanForActors", Random.Range(0f, 0.5f), 0.5f);
        InvokeRepeating("ScanForSkills", Random.Range(0f, 0.5f), 0.05f);
    }

    void ScanForWalls()
    {
        WallVMap.Clear();
        for(int i = 0; i < WallVMap.Count; i++)
        {
            Vector2 dir = WallVMap.GetDir(i);
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.1f, dir, _wallSenseDist);
            //Array.Sort(hits, (x, y) =>
            //{
            //    return (int)Mathf.Sign(Vector2.Distance(x.transform.position, transform.position) - Vector2.Distance(y.transform.position, transform.position));
            //});

            float minHitDist = float.MaxValue;
            List<RaycastHit2D> hitList = new List<RaycastHit2D>(hits);
            for (int k = 0; k < hitList.Count; k++)
            {
                // Ignore Trigger
                if (hitList[k].collider.isTrigger)
                {
                    hitList.RemoveAt(k);
                    k--;
                    continue;
                }

                // Ignore Actor
                Actor actor = hitList[k].collider.GetComponentInParent<Actor>();
                if (actor != null)
                {
                    hitList.RemoveAt(k);
                    k--;
                    continue;
                }

                //
                float hitDist = hitList[k].distance;
                if (hitDist < minHitDist) minHitDist = hitDist;
            }


            float dist = hitList.Count > 0 ? minHitDist : _wallSenseDist;
            WallVMap.Map[i] = dir * dist;
        }
    }

    void ScanForActors()
    {
        EnemyActors.Clear();
        AllyActors.Clear();
        List<Actor> actorList = Utils.ComponentScan<Actor>(transform.position, _actorSenseDist, true);
        actorList.ForEach(x =>
        {
            if (x == _actor) return;
            if (_actor.IsEnemyOf(x))
            {
                if(!EnemyActors.Contains(x))EnemyActors.Add(x);
            }
            else
            {
                if (!AllyActors.Contains(x)) AllyActors.Add(x);
            }
        });
    }

    void ScanForSkills()
    {
        EnemySkills.Clear();
        AllySkills.Clear();
        List<SkillInstance> skillInstanceList = Utils.ComponentScan<SkillInstance>(transform.position, _skillSenseDist);
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
