using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : PersistantSingleton<PlayerManager>
{
    [SerializeField] List<string> _joinSceneNames = new();
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

        UpdateCanJoin();
    }

    private void OnLevelWasLoaded(int level)
    {
        UpdateCanJoin();
    }

    void UpdateCanJoin()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        if (_joinSceneNames.Contains(sceneName))
        {
            _playerInputManager.EnableJoining();
        }
        else
        {
            _playerInputManager.DisableJoining();
        }
    }
}
