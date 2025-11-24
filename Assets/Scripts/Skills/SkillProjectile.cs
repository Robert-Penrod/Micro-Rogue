using HyperQuest.EasyPooling;
using UnityEngine;

public class SkillProjectile : MonoBehaviour, IPoolable
{
    float _lifePercent;
    float _initRot;

    Rigidbody2D _rb;

    public void Initialize()
    {
        _rb  = GetComponent<Rigidbody2D>();
        _rb.AddForce(Vector2.up * Constants.SkillStats.Speed.Default, ForceMode2D.Impulse);
        transform.up = _rb.linearVelocity;
        _initRot = transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        float lastPercent = _lifePercent;
        _lifePercent += Time.deltaTime / (Constants.SkillStats.Duration.Melee * 2f);
        if (_lifePercent >= 1)
        {
            gameObject.DestroyOrRecycle();
        }
    }
}
