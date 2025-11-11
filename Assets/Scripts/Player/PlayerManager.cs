using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : PersistantSingleton<PlayerManager>
{
    public List<Player> PlayerList = new();

    PlayerInputManager _playerInputManager;

    protected override void Awake()
    {
        // Init
        base.Awake();
        _playerInputManager = GetComponent<PlayerInputManager>();

        // Player Joined
        _playerInputManager.onPlayerJoined += (PlayerInput playerInput) =>
        {
            Player player = playerInput.GetComponentInParent<Player>();
            if (player == null || PlayerList.Contains(player)) return;
            PlayerList.Add(player);
        };

        // Player Left
        _playerInputManager.onPlayerLeft += (PlayerInput playerInput) =>
        {
            Player player = playerInput.GetComponentInParent<Player>();
            if (player == null || !PlayerList.Contains(player)) return;
            PlayerList.Remove(player);
        };
    }
}
