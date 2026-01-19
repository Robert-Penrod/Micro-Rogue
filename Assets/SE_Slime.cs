using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Skill))]
public class SE_Slime : MonoBehaviour
{
    [SerializeField] AudioClip _hitSound;
    public float Inheritance = 0.1f;
    public float Drag = 0.1f;
    Rigidbody2D _rb;
    Skill _skill;

    List<Rigidbody2D> _colBodyList = new();
    List<Actor> _colActorList = new();
    Dictionary<Actor, float> _actorHitTime = new();

    //float _hitboxDelay => 0.5f * Constants.SkillStats.HitboxDelay;
    float _skillHitDelay;

    Actor _actor;

    private void Start()
    {
        _skill = GetComponent<Skill>();
        _skillHitDelay = 1f / _skill.Stats.Rate.Value;
        Init();
    }

    private void OnEnable()
    {
        Init();   
    }

    void Init()
    {
        _rb = GetComponentInParent<Rigidbody2D>();
        _actor = GetComponentInParent<Actor>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var colBody = collision.GetComponent<Rigidbody2D>();
        if (_colBodyList.Contains(colBody)) _colBodyList.Remove(colBody);

        var actor = collision.GetComponent<Actor>();
        if (actor != null && _colActorList.Contains(actor))
        {
            _colActorList.Remove(actor);
        }
    }

    void HandleCollision(Collider2D col)
    {
        var colBody = col.GetComponent<Rigidbody2D>();
        if (colBody != null)
        {
            if (!_colBodyList.Contains(colBody)) _colBodyList.Add(colBody);

            var actor = colBody.GetComponent<Actor>();
            if (actor != null && !_colActorList.Contains(actor))
            {
                if (_actor.IsEnemyOf(actor))
                {
                    _colActorList.Add(actor);
                }
            }
        }

    }

    private void FixedUpdate()
    {
        // Hits
        _colActorList.ForEach(actor =>
        {
            if(!_actorHitTime.ContainsKey(actor) ? true : Time.time - _actorHitTime[actor] > _skillHitDelay)
            {
                DoHit(actor);
            }
        });

        // Physics
        _colBodyList.ForEach(colBody =>
        {
            colBody.linearVelocity *= (1f - Drag * Time.fixedDeltaTime);
            colBody.linearVelocity += Inheritance * _rb.linearVelocity;
        });
    }

    void DoHit(Actor actor)
    {
        int damageTaken = actor.TakeDamage((int)_skill.Stats.Damage.Value, null, _actor);
        if (!_actorHitTime.ContainsKey(actor)) _actorHitTime.Add(actor, Time.time);
        else _actorHitTime[actor] = Time.time;

        Vector2 hitForce = 1f * (actor.transform.position - transform.position).normalized * _skill.Stats.Knockback.Value;
        actor.Body.AddDecayForce(hitForce);

        PlayAudio(_hitSound, damageTaken / _skill.Stats.Damage.Value);
    }

    void PlayAudio(AudioClip clip, float volMult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.2f, 1f, volMult);
    }
}
