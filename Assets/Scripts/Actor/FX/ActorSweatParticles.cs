using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ActorSweatParticles : MonoBehaviour
{
    ParticleSystem _pSystem;
    ActorMoveController _moveController;
    float _initRate;

    private void Awake()
    {
        _pSystem = GetComponent<ParticleSystem>();
        _moveController = GetComponentInParent<ActorMoveController>();

        var emission = _pSystem.emission;
        _initRate = emission.rateOverTime.constant;
        emission.rateOverTime = 0f;
    }

    private void Update()
    {
        var emission = _pSystem.emission;
        float rateMult = _moveController.DodgeCooldownPercent.RemapPercent(1f, 0.5f);
        if (_moveController.DodgeCooldownPercent >= 0.9f) rateMult *= 0f;
        emission.rateOverTime = rateMult * _initRate;
    }
}
