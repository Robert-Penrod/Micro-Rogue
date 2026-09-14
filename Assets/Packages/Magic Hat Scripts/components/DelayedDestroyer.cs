using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroyer : MonoBehaviour
{
    public float DestroyTime;
    float _destroyTick = 0f;
    public float DestroyTimeRand;

    public float CompletionPercent { get; private set; }
    public float Lifetime => _destroyTick;

    bool _isDone = false;

    private void Awake()
    {
        _destroyTick = 0f;
        DestroyTime += Random.Range(-DestroyTimeRand, DestroyTimeRand);
    }

    private void Update()
    {
        if (_isDone) return;

        _destroyTick += Time.deltaTime;
        if(_destroyTick > DestroyTime)
        {
            Destroy(this.gameObject);
        }

        CompletionPercent = DestroyTime > 0? _destroyTick / DestroyTime : 1f;
    }
}
