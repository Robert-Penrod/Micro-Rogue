using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ActorWalkParticles : MonoBehaviour
{
    ParticleSystem _pSystem;
    ActorWalkFX _actorWalkFX;
    Rigidbody2D _body;

    float _pMinSpeed;
    float _pMaxSpeed;

    private void Awake()
    {
        // Get References
        _pSystem = GetComponent<ParticleSystem>();
        _actorWalkFX = GetComponentInParent<Actor>().GetComponentInChildren<ActorWalkFX>();
        _body = GetComponentInParent<Rigidbody2D>();

        // Cache pSystem values
        ParticleSystem.MainModule main = _pSystem.main;
        _pMinSpeed = main.startSpeed.constantMin;
        _pMaxSpeed = main.startSpeed.constantMax;

        // Handle Step
        _actorWalkFX.OnStep += () => HandleStep();

        
    }

    void OnLevelChanged(int oldLvl, int newLvl)
    {
        _pSystem.Clear();
    }

    private void OnDestroy()
    {
        ParticleSystem.MainModule main = _pSystem.main;
        _pSystem.Stop();
        main.stopAction = ParticleSystemStopAction.Destroy;
    }

    void HandleStep()
    {
        // Get info
        float speed = _body.linearVelocity.magnitude;
        float pSystemSpeedMult = speed / 5f;

        // Set pSystem speed
        ParticleSystem.MainModule main = _pSystem.main;
        float min = _pMinSpeed * pSystemSpeedMult;
        float max = _pMaxSpeed * pSystemSpeedMult;
        main.startSpeed = new ParticleSystem.MinMaxCurve(min, max);

        // trigger burst
        _pSystem.Play();
    }
}
