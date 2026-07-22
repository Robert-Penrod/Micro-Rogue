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
        public int Index;
        public Color Color;
        public int Gold
        {
            get
            {
                return PlayerPrefs.GetInt($"{Index}_Gold", 0);
                //return _coin;
            }
            set
            {
                PlayerPrefs.SetInt($"{Index}_Gold", value);
                _gold = value;
                OnCoinChange?.Invoke();
            }
        }
        int _gold;
        public Action OnCoinChange;
        public static int Gems
        {
            get
            {
                return PlayerPrefs.GetInt("Gems", 0);
                //return _gem;
            }
            set
            {
                _gem = value;
                PlayerPrefs.SetInt("Gems", _gem);
                OnGemChange?.Invoke();
            }
        }
        public static int _gem;
        public static Action OnGemChange;

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
            return PlayerPrefs.GetString("UnlockedSkills", "Sword, Dagger, Fireball, Vitality, Helmet, Swift Boots, Growth Tome, Fruit, Steak, Fairy In a Bottle, Coffee, Compass, Map, Coinpurse");
        }
    }
    public PlayerData Data;

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
        Submit = PlayerInput.actions["Submit"];

        Actor = GetComponentInChildren<Actor>(true);
        Data.Index = PlayerManager.I?.PlayerList?.Count-1 ?? -1;
        Data.Color = PlayerManager.I.GetPlayerColor(Data.Index);

        // Place Actor in spawn pos
        Vector2 spawnPos =  SpawnSystem.GetRandomEmptyPos(2f);
        Actor.transform.position = (Vector3)spawnPos + Vector3.forward * Actor.transform.position.z;
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
