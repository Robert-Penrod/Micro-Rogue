using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    PlayerInput _playerInput;
    private InputAction _leave;

    public Actor Actor { get; private set; }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _leave = _playerInput.actions.FindAction("Leave", false);
        if(_leave != null) _leave.performed += Disconnect;

        Actor = GetComponentInChildren<Actor>();
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
