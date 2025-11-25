using HyperQuest.EasyPooling;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SIE_Projectile : SIE, IPoolable
{
    float _speed => _skillInstance.Skill.Stats.Speed;
    Rigidbody2D _rb;

    protected override void Awake()
    {
        base.Awake();

        _rb = GetComponent<Rigidbody2D>();

        _skillInstance.OnActivated += () =>
        {
            Launch();
        };
    }

    public void Initialize()
    {
        gameObject.SetCollidersEnabled2D(false);
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Launch()
    {
        Debug.Log("Launch");
        transform.SetParent(null);
        gameObject.SetCollidersEnabled2D(true);
        _rb.bodyType = RigidbodyType2D.Dynamic;

        Vector2 launchForce = _speed * transform.up;

        var actor = _skillInstance.Skill.Actor;
        if (actor != null)
        {
            Vector2 projectedParentVel = Vector3.Project(actor.Body.linearVelocity, launchForce.normalized);
            launchForce += projectedParentVel * Constants.SkillStats.SIE_ProjectileInheritVelocityMult;
        }

        _rb.AddForce(launchForce, ForceMode2D.Impulse);
    }
}
