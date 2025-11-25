using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] AudioClip _enterSound;
    [SerializeField] AudioClip _exitSound;
    float _grabForce = 2f;
    List<Player> _grabbedPlayers = new();
    Dictionary<Player, float> _leaveLog = new();

    float _lerpRot;

    private void Update()
    {
        float targetScale = 0.8f;
        float targetRot = 0f;
        if (this == DungeonManager.I.SelectedPortal)
        {
            // Scale
            targetScale *= DungeonManager.I.PortalPercent.RemapPercent(1f, 1.375f); // PortalPercent mult
            
            // Rotation
            targetRot = 1.5f * Time.deltaTime * 360f * DungeonManager.I.PortalPercent;
        }

        // Scale
        targetScale *= PlayerManager.I.PlayerList.Count > 0 ? ((float)_grabbedPlayers.Count / PlayerManager.I.PlayerList.Count).RemapPercent(1f, 1.375f) : 1;
        float lerpScale = transform.localScale.x.Lerp(targetScale, 3f * Time.deltaTime);
        transform.localScale = Vector3.one * lerpScale;

        // Rotation
        _lerpRot = _lerpRot.Lerp(targetRot, 6f * Time.deltaTime);
        transform.GetChild(0).Rotate2D(_lerpRot);
    }

    private void FixedUpdate()
    {
        for(int i = 0; i < _grabbedPlayers.Count; i++)
        {
            // Info
            float dist = Vector2.Distance(transform.position, _grabbedPlayers[i].Actor.transform.position);

            // Grab Force
            float distMult = dist.Remap(0.125f, 1f, 0f, 1f);
            Vector2 towardsCore = transform.position - _grabbedPlayers[i].Actor.transform.position;
            Vector2 grabForceVector = distMult * _grabForce * towardsCore.normalized;
            _grabbedPlayers[i].Actor.Body.AddForce(grabForceVector * _grabbedPlayers[i].Actor.Body.linearDamping);

            // Slide Force
            _grabbedPlayers[i].Actor.Body.AddForce(0.5f * _grabbedPlayers[i].Actor.Body.linearVelocity * _grabbedPlayers[i].Actor.Body.linearDamping);

            // Dampening
            _grabbedPlayers[i].Actor.Body.linearVelocity *= 0.5f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check Tag
        if (!collision.CompareTag("Player")) 
            return;

        // Check Player 
        var player = collision.GetComponentInParent<Player>();
        if (player == null || _grabbedPlayers.Contains(player))
            return;

        // Check leaveLog
        if (_leaveLog.ContainsKey(player))
        {
            if (Time.time - _leaveLog[player] < 0.5f) return;
        }

        // Add player
        _grabbedPlayers.Add(player);
        player.Actor.SetInPortal(true);

        HandlePlayerEnter(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var actor = collision.GetComponent<Actor>();
        if (actor == null) return;
        var player = actor.GetComponentInParent<Player>();
        if (player != null && _grabbedPlayers.Contains(player))
        {
            _grabbedPlayers.Remove(player);

            // Log
            if (_leaveLog.ContainsKey(player)) _leaveLog[player] = Time.time;
            else _leaveLog.Add(player, Time.time);

            player.Actor.SetInPortal(false);

            // Kick
            Vector2 kickVector = ((Vector2)(player.Actor.transform.position - transform.position)).normalized;
            kickVector *= 0.125f * _grabForce;
            player.Actor.Body.AddForce(kickVector * player.Actor.Body.linearDamping, ForceMode2D.Impulse);
            Debug.DrawLine(transform.position, transform.position + (Vector3)kickVector, Color.green, 1f);

            HandlePlayerExit(player);
        }
    }

    void HandlePlayerEnter(Player player)
    {
        player.SelectedPortal = this;
        AudioSpawner.PlayAudioWithRandPitch(_enterSound, 0.2f, 1.25f, 1f);
    }

    void HandlePlayerExit(Player player)
    {
        player.SelectedPortal = null;
        AudioSpawner.PlayAudioWithRandPitch(_exitSound, 0.2f, 0.75f, 1f);
    }
}
