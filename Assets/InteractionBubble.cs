using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractionBubble : MonoBehaviour
{
    [SerializeField] AudioClip _interactCompleteSound;
    [SerializeField] AudioClip _interactStartSound;
    [SerializeField] float _interactTime = 1.5f;
    [SerializeField] float _interactCooldownTime = 1f;
    float _interactCooldownTick;

    public bool IsInteractable = true;
    [SerializeField] SpriteRenderer _mainSprite;
    [SerializeField] ParticleSystem _pSystem;
    List<Player> _colPlayerList = new List<Player>();
    float _interactTick;
    float _interactPercent => _interactTick / _interactTime;

    [SerializeField] SpriteRenderer _interactMeterBG;
    [SerializeField] SpriteRenderer _interactMeterFill;

    public UnityEvent OnInteract;

    private void Start()
    {
        transform.GetChild(0).localScale = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleCollision(collision, false);
    }

    void HandleCollision(Collider2D col, bool isEntering)
    {
        var colActor = col.GetComponentInParent<Actor>();
        if (colActor == null) return;

        var colPlayer = colActor.GetComponentInParent<Player>();
        if (colPlayer == null) return;

        // Player List
        if (isEntering && !_colPlayerList.Contains(colPlayer)) _colPlayerList.Add(colPlayer);
        else if (!isEntering && _colPlayerList.Contains(colPlayer)) _colPlayerList.Remove(colPlayer);
    }

    public List<Player> GetInteractingPlayers()
    {
        if (!IsInteractable) return null;
        if (_interactCooldownTick > 0f) return null;
        if (_colPlayerList.Count == 0) return _colPlayerList;
        return new List<Player>() { _colPlayerList[0] };
    }

    public List<Player> GetRepulsedPlayers()
    {
        var list = new List<Player>(_colPlayerList);
        var interactionPlayers = GetInteractingPlayers();
        if (interactionPlayers == null) return list;
        list.RemoveAll(player => interactionPlayers.Contains(player));
        return list;
    }

    private void Update()
    {
        bool isInteractable = IsInteractable && _interactCooldownTick <= 0f;

        if(_interactCooldownTick > 0f)
        {
            _interactCooldownTick -= Time.deltaTime;
        }

        bool isBeingInteracted = (GetInteractingPlayers()?.Count ?? 0) > 0;

        if(isBeingInteracted)
        {
            if(_interactTick <= 0f)
            {
                //PlayAudio(_interactStartSound, 0.5f);
                var actorSFX = GetInteractingPlayers()[0].Actor.GetComponentInChildren<ActorSFX>();
                if(actorSFX != null)
                {
                    actorSFX.PlayAudio(0.5f);
                }
            }

            float prevInteractTick = _interactTick;
            _interactTick += Time.deltaTime;
            if (_interactTick >= _interactTime)
            {
                _interactTick = _interactTime;
                if(prevInteractTick < _interactTime)
                {
                    // New Interact
                    OnInteract?.Invoke();
                    _interactCooldownTick = _interactCooldownTime;
                    if(_interactCooldownTime > 0f)
                    {
                        isBeingInteracted = false;
                    }
                    PlayAudio(_interactCompleteSound);
                }
            }
        }
        else
        {
            _interactTick = 0f;
        }

        _interactMeterBG.color = _interactMeterBG.color.Alpha(_interactMeterBG.color.a.Lerp(_interactPercent.Remap(0f, 0.25f, 0f, 0.25f), 6f * Time.deltaTime));
        _interactMeterFill.transform.localScale = Vector3.one * _interactPercent;


        // Scale
        float s = isBeingInteracted? 1.25f : 1f;
        transform.GetChild(0).localScale = transform.GetChild(0).localScale.x.Lerp(s, 6f * Time.deltaTime) * Vector3.one;

        // Color
        Color interactColor = isBeingInteracted? GetInteractingPlayers()[0].Data.Color : Color.blue.Lerp(Color.white, 0.675f);
        Color readyColor = Color.white;
        Color notInteractableColor = Color.red.Lerp(Color.white, 0.675f);
        Color c = (isInteractable ? (isBeingInteracted? interactColor : readyColor) : notInteractableColor);

        // Sprite
        _mainSprite.color = c.Alpha(_mainSprite.color.a);

        Color d = c.Lerp(Color.white, 0.5f);
        _interactMeterBG.color = d.Alpha(_interactMeterBG.color.a);
        _interactMeterFill.color = d.Alpha(_interactMeterFill.color.a);

        // Particles
        var main = _pSystem.main;
        var emisson = _pSystem.emission;
        main.startColor = c.Alpha(main.startColor.color.a);
        float emissionMult = isInteractable ? 1f : 0.5f;
        emissionMult *= isBeingInteracted ? 3f : 1f;
        emisson.rateOverTime = new(emissionMult * 0.5f, emissionMult * 2f);

        float simSpeed = 1f;
        simSpeed *= isInteractable ? 1f : 0.5f;
        simSpeed *= isBeingInteracted ? 2f : 1f;
        main.simulationSpeed = simSpeed;
    }

    void PlayAudio(AudioClip clip, float mult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.2f, 1f, mult);
    }

    private void FixedUpdate()
    {
        GetInteractingPlayers()?.ForEach(player =>
        {
            Attract(player.Actor.Body);
        });

        GetRepulsedPlayers()?.ForEach(player =>
        {
            Repulse(player.Actor.Body);
        });
    }

    void Attract(Rigidbody2D body)
    {
        // info
        Vector2 dir = transform.position - body.transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        // Grab
        float r = 0.5f * transform.lossyScale.x;
        float distMult = dist.Remap(0.75f * r, 1f * r, 0f, 1f);
        Vector2 forceVector = 0.5f * distMult * Constants.ActorStats.MoveSpeed.Default * dir;
        body.AddDampForce(forceVector);

        // Slide
        //body.AddForce(0.5f * body.linearVelocity);

        // Dampening
        //body.linearVelocity = body.linearVelocity * (1f - (2f * Time.fixedDeltaTime));
    }

    void Repulse(Rigidbody2D body)
    {
        Vector2 dir = body.transform.position - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();
        Vector2 forceVector = 0.75f * dir * Constants.ActorStats.MoveSpeed.Default;
        body.AddDampForce(forceVector);
    }
}
