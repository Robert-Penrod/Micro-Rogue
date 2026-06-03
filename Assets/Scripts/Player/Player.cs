using System;
using System.Collections.Generic;
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

        public static List<string> GetUnlockedSkillList()
        {
            return new(GetUnlockedSkillString().Split(", "));
        }
        public static void UnlockSkill(string skillName)
        {
            PlayerPrefs.SetString("UnlockedSkills", GetUnlockedSkillString() + ", " + skillName);
        }
        static string GetUnlockedSkillString()
        {
            return PlayerPrefs.GetString("UnlockedSkills", "all, Sword, Dagger, Fireball, Vitality, Helmet, Swift Boots, Growth Tome");
        }
    }
    public PlayerData Data;

    public int Index { get; private set; }

    public PlayerInput PlayerInput { get; private set; }
    InputAction _leave;
    public InputAction Submit { get; private set; }

    public Actor Actor { get; private set; }

    public Portal SelectedPortal;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        _leave = PlayerInput.actions.FindAction("Leave", false);
        if(_leave != null) _leave.performed += Disconnect;
        Submit = PlayerInput.actions["Dash"];

        Actor = GetComponentInChildren<Actor>(true);
        Index = PlayerManager.I?.PlayerList?.Count-1 ?? -1;
        Data.Color = PlayerManager.I.GetPlayerColor(Index);

        PlayerData.Gem = PlayerPrefs.GetInt("Gems", 0);
    }

    private void Update()
    {
        if(PlayerInput.actions.FindAction("Dash").IsPressed()) Actor.MoveController.Ctrl_Dodge(Actor.MoveController.MoveDir);
    }

    private void FixedUpdate()
    {
        if (Actor == null || Actor.MoveController == null) return;
        Actor.MoveController.Ctrl_Move(PlayerInput.actions.FindAction("Move").ReadValue<Vector2>());
    }

    void OnDestroy()
    {
        if (_leave != null) _leave.performed -= Disconnect;
    }

    public void Disconnect(InputAction.CallbackContext ctx)
    {
        PlayerInput.DeactivateInput();
        if (PlayerInput.user.valid) PlayerInput.user.UnpairDevicesAndRemoveUser();
        if(gameObject != null) Destroy(gameObject);
    }
}
