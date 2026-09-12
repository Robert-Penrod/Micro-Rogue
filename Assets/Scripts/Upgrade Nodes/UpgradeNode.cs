using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    public class UpgradeNodeStatUpgrade
    {
        public SkillStats.SkillStatTypes SkillStatType;
        public ActorStats.ActorStatTypes ActorStatType;
        public float FlatBonus;
    }

    [Header("Link")]
    [SerializeField] List<UpgradeNode> _childNodes = new();

    [Header("Unlock")]
    [SerializeField] string UpgradeString;
    [SerializeField] UpgradeNodeStatUpgrade _unlockedStat;
    [SerializeField] Skill _unlockedSkill;
    [SerializeField] Actor _unlockedChar;

    [Header("Cost")]
    [SerializeField] public int _gemCost;

    [Header("Params")]
    [SerializeField] string _name;
    [SerializeField] int _maxUpgrades = 1;
    [SerializeField] Sprite _sprite;
    [SerializeField] Color _color = Color.clear;

    [Header("References")]
    [SerializeField] Image _iconRef;
    [SerializeField] Image _outlineRef;
    [SerializeField] GameObject _costHolder;
    [SerializeField] TextMeshProUGUI _costText;

    bool IsRevealed = true;
    bool IsUnlocked = false;
    bool CanAfford()
    {
        return Player.PlayerData.Gems >= _gemCost;
    }

    private void OnValidate()
    {
        UpdateGFX();

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
            List<UpgradeNode> frontier = new();
            List<UpgradeNode> nextFrontier = new();
            frontier.AddRange(_childNodes);
            int depth = 2;

            while (frontier.Count > 0)
            {
                nextFrontier.Clear();
                frontier.ForEach(node =>
                {
                    node._gemCost = (int)Mathf.Pow(2, depth);
                    node.UpdateGFX();
                    Debug.Log(node._gemCost);
                    nextFrontier.AddRange(node._childNodes);
                });
                frontier.Clear();
                frontier.AddRange(nextFrontier);
                depth++;
            }
        }

        IsRevealed = LoadBool("Revealed");
        IsUnlocked = LoadBool("Unlocked");
        UpdateGFX();

        Player.PlayerData.OnGemChange += OnGemUpdate;
    }

    private void OnDestroy()
    {
        Player.PlayerData.OnGemChange -= OnGemUpdate;
    }

    void OnGemUpdate(int delta)
    {
        UpdateGFX();
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
        UpdateGFX();
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

    void UpdateGFX()
    {
        transform.GetChild(0).gameObject.SetActive(IsRevealed);
        if (!IsRevealed) return;

        // Cost
        _costHolder.SetActive(!IsUnlocked);
        _costText.text = $"-{_gemCost}";

        // Icon
        _iconRef.enabled = false;
        if (_sprite != null)
        {
            _iconRef.enabled = true;
            _iconRef.sprite = _sprite;
            if (_color != Color.clear) _iconRef.color = _color;
        }
        if (_unlockedSkill != null)
        {
            _iconRef.enabled = true;
            _iconRef.sprite = _unlockedSkill.Icon;
            _iconRef.color = _unlockedSkill.GetColor().SetValue(1f);
        }
        if (_unlockedChar != null)
        {
            _iconRef.enabled = true;
            _iconRef.sprite = _unlockedChar._spriteRend.sprite;
            _iconRef.color = _unlockedChar.Color;
        }
        _iconRef.gameObject.SetActive(_iconRef.enabled);

        // Frame
        if(IsUnlocked)
        {
            _outlineRef.color = Color.yellow;
        }
        else
        {
            _outlineRef.color = CanAfford() ? Color.white : Color.red;
        }       

        this.DelayedInvoke(-1, () =>
        {
            UpdateLines();
        });
    }

    void UpdateLines()
    {
        float zOffset = -50f;

        List<LineRenderer> lineRends = new(GetComponentsInChildren<LineRenderer>());
        _childNodes.RemoveAll(x => x == null);

        for (int childIndex = 0; childIndex < _childNodes.Count; childIndex++)
        {
            var childNode = _childNodes[childIndex];
            LineRenderer lineRend = null;
            if (lineRends.Count > childIndex)
            {
                lineRend = lineRends[childIndex];
            }
            else
            {
                lineRend = new GameObject($"Line Rend {childIndex}").AddComponent<LineRenderer>();
                lineRend.transform.SetParent(transform);
                lineRends.Add(lineRend);
            }

            lineRend.widthMultiplier = 0.13f;
            lineRend.material = Canvas.GetDefaultCanvasMaterial();
            lineRend.positionCount = 2;
            lineRend.SetPosition(0, ((RectTransform)transform).position + Vector3.forward * zOffset);
            lineRend.SetPosition(1, ((RectTransform)childNode.transform).position + Vector3.forward * zOffset);

            lineRend.gameObject.SetActive(childNode.IsRevealed);
        }

        // Remove excess lines
        while(lineRends.Count > _childNodes.Count)
        {
            Destroy(lineRends[_childNodes.Count].gameObject);
            DestroyImmediate(lineRends[_childNodes.Count].gameObject);
            lineRends.RemoveAt(_childNodes.Count);
        }
    }

    public string GetDescription()
    {
        return "(not implimented)";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down??");
        UnlockNode();
        UpdateGFX();
    }
}
