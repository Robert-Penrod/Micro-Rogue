using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ActorWalkParticles : MonoBehaviour
{
    [SerializeField] bool _doesDash = false;
    ParticleSystem _pSystem;
    ActorWalkFX _actorWalkFX;
    Rigidbody2D _body;
    Actor _actor;

    float _pMinSpeed;
    float _pMaxSpeed;

    private void Awake()
    {
        // Get References
        _actor = GetComponentInParent<Actor>();
        _pSystem = GetComponent<ParticleSystem>();
        _actorWalkFX = _actor?.GetComponentInChildren<ActorWalkFX>();
        _body = GetComponentInParent<Rigidbody2D>();

        // Cache pSystem values
        ParticleSystem.MainModule main = _pSystem.main;
        _pMinSpeed = main.startSpeed.constantMin;
        _pMaxSpeed = main.startSpeed.constantMax;

        // Handle Step
        if (_actorWalkFX != null)
        {
            _actorWalkFX.OnStep += () => HandleStep();
        }

        if (_doesDash)
        {
            _actor.MoveController.OnDodge += () =>
            {
                HandleDodge();
            };
        }
    }

    void HandleDodge()
    {
        StartCoroutine(HandleDodge_Co());
        IEnumerator HandleDodge_Co()
        {
            while(_actor.MoveController.IsDodging)
            {
                HandleStep();
                yield return new WaitForSeconds(0.05f);
            }
        }
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

    void HandleStep(float mult = 1f)
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
