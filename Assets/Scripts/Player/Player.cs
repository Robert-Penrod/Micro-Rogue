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
                return _gold;// PlayerPrefs.GetInt($"{Index}_Gold", 0);
                //return _coin;
            }
            set
            {
                //PlayerPrefs.SetInt($"{Index}_Gold", value);
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
                var oldValue = _gem;
                _gem = value;
                PlayerPrefs.SetInt("Gems", _gem);
                OnGemChange?.Invoke(_gem - oldValue);
            }
        }
        public static int _gem;
        public static Action<int> OnGemChange;

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
            return PlayerPrefs.GetString("UnlockedSkills", "all, Sword, Dagger, Fireball, Vitality, Helmet, Swift Boots, Growth Tome, Fruit, Steak, Fairy In a Bottle, Coffee, Compass, Map, Coinpurse");
        }
    }
    public PlayerData Data;

    public static class CampUpgradeData
    {
        public static Action OnCampDataChange;

        static int _startingSlotCount = (DemoManager.I?.IsDemo ?? false) ? 1 : 0;

        public static int SkillSlotLevel
        {
            get
            {
                return PlayerPrefs.GetInt("SkillSlotUpgradeCount", _startingSlotCount);
            }
            set
            {
                PlayerPrefs.SetInt("SkillSlotUpgradeCount", value);
                OnCampDataChange?.Invoke();
            }
        }
        public static int SkillSlotCount => 1 + SkillSlotLevel;

        public static int PassiveSlotUpgradeCount
        {
            get
            {
                return PlayerPrefs.GetInt("PassiveSlotUpgradeCount", _startingSlotCount);
            }
            set
            {
                PlayerPrefs.SetInt("PassiveSlotUpgradeCount", value);
                OnCampDataChange?.Invoke();
            }
        }
        public static int PassiveSlotCount => 1 + PassiveSlotUpgradeCount;

        public static int ItemSlotUpgradeCount
        {
            get
            {
                return PlayerPrefs.GetInt("ItemSlotUpgradeCount", _startingSlotCount);
            }
            set
            {
                PlayerPrefs.SetInt("ItemSlotUpgradeCount", value);
                OnCampDataChange?.Invoke();
            }
        }
        public static int ItemSlotCount => 0;// 1 + ItemSlotUpgradeCount;

        public static int LootLevel
        {
            get
            {
                return PlayerPrefs.GetInt("LootLevel", 0);
            }
            set
            {
                PlayerPrefs.SetInt("LootLevel", value);
                OnCampDataChange?.Invoke();
            }
        }
        public static float LootMultiplier => 1f + (0.12f * LootLevel);

        public static int HordeLevel
        {
            get
            {
                return PlayerPrefs.GetInt("HordeLevel", 0);
            }
            set
            {
                PlayerPrefs.SetInt("HordeLevel", value);
                OnCampDataChange?.Invoke();
            }
        }
        public static int HordeStartingGold => 20 * HordeLevel;
    }


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
