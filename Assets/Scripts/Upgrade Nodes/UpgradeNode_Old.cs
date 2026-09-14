using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeNode_Old : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    public class UpgradeNodeStatUpgrade
    {
        public SkillStats.SkillStatTypes SkillStatType;
        public ActorStats.ActorStatTypes ActorStatType;
        public float FlatBonus;
    }

    [Header("Link")]
    public List<UpgradeNode_Old> _childNodes = new();

    [Header("Cost")]
    [SerializeField] public int _gemCost;

    [Header("Unlock")]
    public string UpgradeString;
    public UpgradeNodeStatUpgrade _unlockedStat;
    public Skill _unlockedSkill;
    public Actor _unlockedChar;

    [Header("Params")]
    public string _name;
    public int _maxUpgrades = 1;
    public Sprite _sprite;
    public Color _color = Color.clear;

    [field: SerializeField]
    public bool IsRevealed { get; private set; }
    [field: SerializeField]
    public bool IsUnlocked { get; private set; }
    public bool CanAfford()
    {
        return Player.PlayerData.Gems >= _gemCost;
    }

    public Action OnGFXUpdate;

    void TriggerGFXUpdate()
    {
        OnGFXUpdate?.Invoke();
    }

    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            IsRevealed = true;
        }

        TriggerGFXUpdate();

        if(_unlockedSkill != null)
        {
            this.gameObject.name = "Node_" + _unlockedSkill.Name;
        }
        else if(UpgradeString != string.Empty)
        {
            this.gameObject.name = "Node_" + UpgradeString;
        }
    }

    private void Start()
    {
        // Propogate Cost
        if(GetNodeID() == "0, 0")
        {
            _gemCost = 2;
            Debug.Log("Propgate cost");
            List<UpgradeNode_Old> frontier = new();
            List<UpgradeNode_Old> nextFrontier = new();
            frontier.AddRange(_childNodes);
            int depth = 2;

            while (frontier.Count > 0)
            {
                nextFrontier.Clear();
                frontier.ForEach(node =>
                {
                    node._gemCost = (int)Mathf.Pow(2, depth);
                    node.TriggerGFXUpdate();
                    nextFrontier.AddRange(node._childNodes);
                });
                frontier.Clear();
                frontier.AddRange(nextFrontier);
                depth++;
            }
        }

        IsRevealed = LoadBool("Revealed");
        IsUnlocked = LoadBool("Unlocked");
        TriggerGFXUpdate();

        Player.PlayerData.OnGemChange += OnGemUpdate;
    }

    private void OnDestroy()
    {
        Player.PlayerData.OnGemChange -= OnGemUpdate;
    }

    void OnGemUpdate(int delta)
    {
        TriggerGFXUpdate();
    }

    bool LoadBool(string name)
    {
        string nodeId = GetNodeID();
        bool loadValue = PlayerPrefs.GetInt($"Node_{nodeId}_{name}", 0) > 0;

        if (name == "Revealed" && nodeId == "0, 0") loadValue = true;

        return loadValue;
    }

    void SaveBool(string name, bool value)
    {
        if (name == "Revealed") IsRevealed = value;
        if (name == "Unlocked") IsUnlocked = value;

        PlayerPrefs.SetInt($"Node_{GetNodeID()}_{name}", value ? 1 : 0);
        TriggerGFXUpdate();
    }

    public void UnlockNode()
    {
        if (!CanAfford() || IsUnlocked) return;

        if(_unlockedSkill != null)
        {
            Player.PlayerData.UnlockSkill(_unlockedSkill.Name);
        }
        else if(UpgradeString != string.Empty)
        {
            if(UpgradeString == "MainSlot")
            {
                Player.CampUpgradeData.MainSlotCount++;
            }
            else if(UpgradeString == "PassiveSlot")
            {
                Player.CampUpgradeData.PassiveSlotCount++;
            }
        }

        Player.PlayerData.Gems -= _gemCost;
        SaveBool("Unlocked", true);
        _childNodes.ForEach(child =>
        {
            child.SaveBool("Revealed", true);
        });
    }

    string GetNodeID()
    {
        return ((int)transform.position.x).ToString() + ", " + ((int)transform.position.y).ToString();
    }

    public string GetDescription()
    {
        return "(not implimented)";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down??");
        UnlockNode();
        TriggerGFXUpdate();
    }
}
