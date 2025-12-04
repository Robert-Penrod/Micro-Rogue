using ManaSprite.EasyPooling;
using UnityEngine;

public class ScentDrop : MonoBehaviour, IPoolable
{
    public float Age => _lifeTick;
    float _scentLifetime => ScentSystem.Lifetime;

    public ScentSystem ScentSystem;
    float _lifeTick = 0f;

    public void Initialize()
    {
        _lifeTick = 0f;
    }

    private void FixedUpdate()
    {
        _lifeTick += Time.fixedDeltaTime;
        if(_lifeTick >= _scentLifetime)
        {
            gameObject.DestroyOrRecycle();
        }
    }
}
