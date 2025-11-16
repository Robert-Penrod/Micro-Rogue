using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    float _grabForce = 3f;
    List<Player> _grabbedPlayers = new();
    Dictionary<Player, float> _leaveLog = new();

    private void FixedUpdate()
    {
        for(int i = 0; i < _grabbedPlayers.Count; i++)
        {
            // Info
            float dist = Vector2.Distance(transform.position, _grabbedPlayers[i].Actor.transform.position);

            // Grab Force
            Debug.Log(dist);
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
        var player = collision.GetComponentInParent<Player>();
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
    }

    void HandlePlayerExit(Player player)
    {
        player.SelectedPortal = null;
    }
}
