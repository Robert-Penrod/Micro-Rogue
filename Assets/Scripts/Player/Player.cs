using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public Color Color;
        public int Coin
        {
            get
            {
                return _coin;
            }
            set
            {
                _coin = value;
                OnCoinChange?.Invoke();
            }
        }
        int _coin;
        public Action OnCoinChange;
        public static int Gem
        {
            get
            {
                return _gem;
            }
            set
            {
                _gem = value;
                PlayerPrefs.SetInt("Gems", _gem);
            }
        }
        public static int _gem;
    }
    public PlayerData Data;

    public int Index { get; private set; }

    PlayerInput _playerInput;
    private InputAction _leave;

    public Actor Actor { get; private set; }

    public Portal SelectedPortal;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _leave = _playerInput.actions.FindAction("Leave", false);
        if(_leave != null) _leave.performed += Disconnect;

        Actor = GetComponentInChildren<Actor>();
        Index = PlayerManager.I?.PlayerList?.Count-1 ?? -1;
        Data.Color = PlayerManager.I.GetPlayerColor(Index);

        PlayerData.Gem = PlayerPrefs.GetInt("Gems", 0);
    }

    private void Update()
    {
        if(_playerInput.actions.FindAction("Dash").IsPressed()) Actor.MoveController.Ctrl_Dodge(Actor.MoveController.MoveDir);
    }

    private void FixedUpdate()
    {
        Actor.MoveController.Ctrl_Move(_playerInput.actions.FindAction("Move").ReadValue<Vector2>());
    }

    void OnDestroy()
    {
        if (_leave != null) _leave.performed -= Disconnect;
    }

    public void Disconnect(InputAction.CallbackContext ctx)
    {
        _playerInput.DeactivateInput();
        if (_playerInput.user.valid) _playerInput.user.UnpairDevicesAndRemoveUser();
        if(gameObject != null) Destroy(gameObject);
    }
}
