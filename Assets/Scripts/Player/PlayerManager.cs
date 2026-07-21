using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] List<string> _joinSceneNames = new();
    public List<Player> PlayerList = new();

    public PlayerInputManager _playerInputManager { get; private set; }

    List<Color> _playerColors = new();

    public Action<Player> OnPlayerJoin;
    public Action<Player> OnPlayerLeave;

    public Player CurrentUIOwner;
    string _uiMapName = "UI";

    public InputDevice LastInputDevice { get; private set; }


    public void JoinPlayerByLastInputDevice()
    {
       _playerInputManager.JoinPlayer(pairWithDevice: LastInputDevice);
    }

    public void GatherPlayersInValidSpace(float size = 1.5f)
    {
        float random = 0.5f;
        Vector2 basePos = SpawnSystem.GetRandomEmptyPos(size);
        PlayerList.ForEach(player =>
        {
            player.transform.position = (Vector3)(basePos + random * Random.insideUnitCircle) + Vector3.forward * player.transform.position.z;
        });
    }

    public void UnjoinAllPlayers()
    {
        for(int i = 0; i < PlayerList.Count; i++)
        {
            if(PlayerList[i] != null)
            {
                Destroy(PlayerList[i].gameObject);
                i--;
            }
        }
    }

    public void SetUIOwner(Player player)
    {
        //Debug.Log("Setting UI Owner");
        this.CurrentUIOwner = player;

        return;

        var uiModule = FindFirstObjectByType<InputSystemUIInputModule>();
        if (!uiModule)
        {
            //Debug.LogError("No InputSystemUIInputModule found in scene.");
            return;
        }

        int ownerIndex = (player == null) ? -1 : PlayerList.IndexOf(player);
        foreach (var pi in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            var uiMap = pi.actions.FindActionMap(_uiMapName, true);

            bool isOwner = player == null || pi.playerIndex == ownerIndex;

            if (isOwner)
            {
                //Debug.Log("Enabling " + pi.playerIndex);
                uiMap.Enable();

                // KEY LINE: now the EventSystem reads THIS player's actions
                uiModule.actionsAsset = pi.actions;
            }
            else
            {
                //Debug.Log("Disabling " + pi.playerIndex);
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

        
        InputSystem.onEvent.Call(eventPtr =>
        {
            /*
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;
            
            // ignore idle stick drift / noise so it only updates on real input
            if (!eventPtr.EnumerateChangedControls(device, magnitudeThreshold: 0.1f).Any()) return;
            */

            var device = InputSystem.GetDeviceById(eventPtr.deviceId);
            if (device == null) return;
            LastInputDevice = device;
        });
        

        // Shuffle Colors
        /*
        Random.InitState(DateTime.Now.GetHashCode());
        _playerColors.Add(Color.HSVToRGB(0f, 0.5f, 1f));
        _playerColors.Add(Color.HSVToRGB(1/4f, 0.5f, 1f));
        _playerColors.Add(Color.HSVToRGB(2/4f, 0.5f, 1f));
        _playerColors.Add(Color.HSVToRGB(3 / 4f, 0.5f, 1f));
        for (int i = 0; i < _playerColors.Count; i++)
        {
            float h = _playerColors[i].GetHue();
            h = h.Lerp(2 / 4f, 0.75f);
            h += 0.01f * Random.Range(-1f, 1f);
            _playerColors[i] = Color.HSVToRGB(h, 0.5f, 1f);
        }
        _playerColors.Shuffle();
        */

        // Init Player Colors
        Color defaultPlayerColor = GamePaletteManager.I.Palette.PlayerColor;
        var defaultPlayerHue = defaultPlayerColor.GetHue();
        defaultPlayerColor = Color.HSVToRGB(defaultPlayerHue, 0.5f, 1f);
        var firstColors = new List<Color>();
        var secondColors = new List<Color>();
        for(int i = 0; i < 8; i++)
        {
            Color c = defaultPlayerColor.SetHue(defaultPlayerHue + i * 0.05f);
            if (i < 4) firstColors.Add(c);
            else secondColors.Add(c);
        }
        firstColors.Shuffle();
        secondColors.Shuffle();
        _playerColors.AddRange(firstColors);
        _playerColors.AddRange(secondColors);

        // Player Join/Leave
        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
        UpdateCanJoin();
    }

    /*
    private void Update()
    {
        PlayerList.ForEach(player => {
            CameraManager.I.Zoom(player.Actor.Body.linearVelocity.magnitude.Remap(0f, 2f * Constants.ActorStats.MoveSpeed.Default, 1f, 1.05f), this);
        });
    }
    */


    public Color GetPlayerColor(int index) => _playerColors[index];

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Player player = playerInput.GetComponentInParent<Player>();
        if (player == null || PlayerList.Contains(player)) return;
        PlayerList.Add(player);
        OnPlayerJoin?.Invoke(player);

        player.transform.position = Random.insideUnitCircle.normalized * Random.Range(0.5f, 1f);
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
        Player.PlayerData.OnGemChange = null;
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
