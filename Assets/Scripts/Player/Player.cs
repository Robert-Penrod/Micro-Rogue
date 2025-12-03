using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public Color Color;
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
    }

    private void FixedUpdate()
    {
        Actor.Move(_playerInput.actions.FindAction("Move").ReadValue<Vector2>());
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
