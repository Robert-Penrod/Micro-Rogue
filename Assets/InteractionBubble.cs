using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionBubble : MonoBehaviour
{
    public bool IsInteractable = true;
    [SerializeField] SpriteRenderer _mainSprite;
    [SerializeField] ParticleSystem _pSystem;
    List<Player> _colPlayerList = new List<Player>();

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
        // Scale
        float s = (_colPlayerList.Count > 0 && IsInteractable)? 1.25f : 1f;
        transform.GetChild(0).localScale = transform.GetChild(0).localScale.x.Lerp(s, 6f * Time.deltaTime) * Vector3.one;

        // Sprite
        _mainSprite.color = (IsInteractable ? Color.white : Color.red.Lerp(Color.white, 0.75f)).Alpha(_mainSprite.color.a);

        // Particles
        var main = _pSystem.main; main.startColor = _mainSprite.color.Alpha(main.startColor.color.a);
        float emissionMult = IsInteractable ? 1f : 0.37f;
        var emisson = _pSystem.emission; emisson.rateOverTime = new(emissionMult * 1f, emissionMult * 3f);
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
