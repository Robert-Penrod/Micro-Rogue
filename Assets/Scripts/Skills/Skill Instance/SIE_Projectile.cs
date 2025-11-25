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
        _rb.AddForce(_speed * transform.up, ForceMode2D.Impulse);
    }
}
