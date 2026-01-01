using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] List<string> _joinSceneNames = new();
    public List<Player> PlayerList = new();

    PlayerInputManager _playerInputManager;

    List<Color> _playerColors = new();

    public Action<Player> OnPlayerJoin;
    public Action<Player> OnPlayerLeave;

    public Player CurrentUIOwner { get; private set; }
    string _uiMapName = "UI";
    public void SetUIOwner(Player player)
    {
        Debug.Log("Setting UI Owner");this.CurrentUIOwner = player;

        var uiModule = FindFirstObjectByType<InputSystemUIInputModule>();
        if (!uiModule)
        {
            Debug.LogError("No InputSystemUIInputModule found in scene.");
            return;
        }

        int ownerIndex = (player == null) ? 0 : PlayerList.IndexOf(player);
        foreach (var pi in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            var uiMap = pi.actions.FindActionMap(_uiMapName, true);

            bool isOwner = player == null || pi.playerIndex == ownerIndex;

            if (isOwner)
            {
                Debug.Log("Enabling " + pi.playerIndex);
                uiMap.Enable();

                // KEY LINE: now the EventSystem reads THIS player's actions
                uiModule.actionsAsset = pi.actions;
            }
            else
            {
                Debug.Log("Disabling " + pi.playerIndex);
                uiMap.Disable();
            }
        }
    }

    public bool AreAllPlayersDead()
    {
        if (PlayerList.Count == 0) return false;

        foreach(var player in PlayerList)
        {
            if(player?.Actor == null) return false;
            if (player.Actor.Stats.Health > 0) return false;
        }

        return true;
    }

    protected override void Awake()
    {
        // Init
        base.Awake();
        _playerInputManager = GetComponent<PlayerInputManager>();

        // Shuffle Colors
        Random.InitState(DateTime.Now.GetHashCode());
        _playerColors.Add(Color.HSVToRGB(0f, 0.5f, 1f));
        _playerColors.Add(Color.HSVToRGB(1/4f, 0.5f, 1f));
        _playerColors.Add(Color.HSVToRGB(2/4f, 0.5f, 1f));
        _playerColors.Add(Color.HSVToRGB(3 / 4f, 0.5f, 1f));
        for (int i = 0; i < _playerColors.Count; i++)
        {
            float h = _playerColors[i].GetHue();
            h = h.Lerp(Color.HSVToRGB(2 / 4f, 0.5f, 1f).GetHue(), 0.5f);
            h += 0.01f * Random.Range(-1f, 1f);
            _playerColors[i] = Color.HSVToRGB(h, 0.5f, 1f);
        }
        _playerColors.Shuffle();

        // Player Join/Leave
        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
        UpdateCanJoin();
    }

    public Color GetPlayerColor(int index) => _playerColors[index];

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Player player = playerInput.GetComponentInParent<Player>();
        if (player == null || PlayerList.Contains(player)) return;
        PlayerList.Add(player);
        OnPlayerJoin?.Invoke(player);
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        Player player = playerInput.GetComponentInParent<Player>();
        if (player == null || !PlayerList.Contains(player)) return;
        PlayerList.Remove(player);
        OnPlayerLeave?.Invoke(player);
    }

    private void OnLevelWasLoaded(int level)
    {
        UpdateCanJoin();
    }

    void UpdateCanJoin()
    {
        var sceneName = SceneManager.GetActiveScene().name;
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
