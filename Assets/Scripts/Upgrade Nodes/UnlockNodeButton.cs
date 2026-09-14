using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SimpleButton))]
public class UnlockNodeButton : MonoBehaviour
{
    #region Vars
    [Header("Link")]
    public bool IsRoot;
    public List<UnlockNodeButton> Children = new();

    [Header("Data")]
    public UnlockNodeData NodeData;
    [System.Serializable]
    public class UnlockNodeData
    {
        #region vars
        [Header("Cost")]
        public int GemCost;

        [Header("Upgrade")]
        public int MaxUpgrades = 1;
        public Skill UnlockedSkill;
        public Actor UnlockedCharacter;
        public StringUnlock UnlockedStringInfo;
        #endregion

        public bool CanAfford()
        {
            return Player.PlayerData.Gems >= GemCost;
        }
    }
    
    [Header("Reference")]
    [SerializeField] Image _icon;
    [SerializeField] Image _frame;
    [SerializeField] Image _bg;
    [SerializeField] TextMeshProUGUI _costText;
    [SerializeField] CanvasGroup _costCGroup;
    SimpleButton _simpleButton;

    public Color _baseBgColor => Color.HSVToRGB(0f, 0f, 0.14f);
    public Color _mainColor => NodeData.UnlockedStringInfo != null ? NodeData.UnlockedStringInfo.Color : (NodeData.UnlockedSkill != null ? NodeData.UnlockedSkill.GetColor().SetValue(1f) : (NodeData.UnlockedCharacter != null ? NodeData.UnlockedCharacter.Color : Color.white));
    Color CantAffordColor = Color.red.SetSaturation(0.7f);
    Color UnlockedColor = Color.yellow;

    float _submitTick;
    float _submitPulseLerp;
    #endregion

    #region Unlock/Load/Save
    public bool UnlockNode()
    {
        if (!NodeData.CanAfford() || IsUnlocked()) return false;

        if (NodeData.UnlockedSkill != null)
        {
            Player.PlayerData.UnlockSkill(NodeData.UnlockedSkill.Name);
        }
        else if (NodeData.UnlockedStringInfo != null)
        {
            switch(NodeData.UnlockedStringInfo.UnlockedString)
            {
                case "Main":
                    Player.CampUpgradeData.MainSlotCount++;
                    break;
                case "Passive":
                    Player.CampUpgradeData.PassiveSlotCount++;
                    break;
                default:
                    Player.UnlockString(NodeData.UnlockedStringInfo.UnlockedString);
                    break;
            }
        }

        Player.PlayerData.Gems -= NodeData.GemCost;
        SaveBool("Unlocked", true);
        Children.ForEach(child =>
        {
            child.SaveBool("Revealed", true);
        });

        UpdateGFX();
        UpdateLines();

        return true;
    }
    public bool IsRevealed() => LoadBool("Revealed");
    public bool IsUnlocked() => LoadBool("Unlocked");
    bool LoadBool(string key)
    {
        if (key.Equals("Revealed")) if (IsRoot) return true;
        return PlayerPrefs.GetInt(GetPlayerPrefsKey(key), 0) > 0 ? true : false;
    }
    void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(GetPlayerPrefsKey(key), value ? 1 : 0);
    }
    string GetPlayerPrefsKey(string key) => $"Node_{GetNodeId()}_{key}";
    string GetNodeId()
    {
        string idString = $"{NodeData.UnlockedStringInfo?.UnlockedString ?? string.Empty}{NodeData.UnlockedSkill?.Name ?? string.Empty}{NodeData.UnlockedCharacter?.name ?? string.Empty}";
        Children.ForEach(child =>
        {
            idString += $"{child.name}{child.NodeData.GemCost}";
        });
        return idString;
    }
    #endregion

    #region Init
    private void OnValidate()
    {
        GetReferences();
        UpdateGFX();

        // Lines
        this.DelayedInvoke(-1, () =>
        {
            UpdateLines();
        });

        // Name
        var unlockedString = NodeData.UnlockedStringInfo;
        var unlockedSkill = NodeData.UnlockedSkill;
        var unlockedChar = NodeData.UnlockedCharacter;
        string name = $"N_{unlockedString?.UnlockedString ?? string.Empty}{unlockedSkill?.Name ?? string.Empty}{unlockedChar?.name ?? string.Empty}";
        this.gameObject.name = name;
    }
    private void Awake()
    {
        GetReferences();
        UpdateGFX();
        UpdateLines();

        _simpleButton.OnSubmit += () =>
        {
            _submitTick += 0.5f;
        };
    }

    private void Start()
    {
        _simpleButton.OnSubmit += () =>
        {
            bool unlocked = UnlockNode();
        };

        if(IsRoot)
        {
            // Propogate Cost
            /*
            List<UnlockNodeButton> children = new();
            List<UnlockNodeButton> frontier = new(Children);
            int depth = 2;
            while (frontier.Count > 0)
            {
                children.Clear();
                foreach (UnlockNodeButton frontierNode in frontier)
                {
                    frontierNode.NodeData.GemCost = (int)2f.Pow(depth);
                    children.AddRange(frontierNode.Children);
                }
                frontier.Clear();
                frontier.AddRange(children);
                depth++;
            }
            */
        }
    }

    void GetReferences()
    {
        _simpleButton = GetComponent<SimpleButton>();
    }
    #endregion

    #region Update
    private void Update()
    {
        {
            if (_submitTick > 0f) _submitTick = (_submitTick - 8f * Time.deltaTime * _submitTick).ClampMin(0);

            // SubmitPulse
            float targetPulseMag = _submitTick;
            float lerpSpeed = 500f;// targetPulseMag > _submitPulseLerp? 16f : 8f;
            _submitPulseLerp = _submitPulseLerp.Lerp(_submitTick, lerpSpeed * Time.deltaTime);
        }

        UpdateGFX();
        //UpdateLines();
    }

    

    void UpdateGFX()
    {
        // Params
        float lerpSpeed = 12f;
        float scaleMult = 1f;
        float hoverMult = 1f;
        scaleMult *= _submitPulseLerp.RemapPercent(1f, 1.5f, false).ClampMin(0);
        bool isUnlocked = IsUnlocked();

        // Enabled
        bool isRevealed = IsRevealed() || !Application.isPlaying;
        _simpleButton.SetActive(isRevealed);
        transform.GetChild(0).gameObject.SetActive(isRevealed);
        if (!isRevealed) return;

        // Icon
        {
            _icon.enabled = false;
            if (NodeData.UnlockedStringInfo != null)
            {
                _icon.enabled = true;
                _icon.sprite = NodeData.UnlockedStringInfo.Sprite;
            }
            if (NodeData.UnlockedSkill != null)
            {
                _icon.enabled = true;
                _icon.sprite = NodeData.UnlockedSkill.Icon;
            }
            if (NodeData.UnlockedCharacter != null)
            {
                _icon.enabled = true;
                _icon.sprite = NodeData.UnlockedCharacter._spriteRend.sprite;
            }
            _icon.gameObject.SetActive(_icon.enabled);
            Color targetColor = _mainColor;
            float damp = 1f;
            damp *= _simpleButton.IsSelected ? 1f : 0.75f;
            damp *= _simpleButton.IsHovered ? 1f + hoverMult * 0.125f : 1f;
            damp *= _submitPulseLerp.RemapPercent(1f, 1.125f, false);
            damp.Clamp01();
            targetColor = targetColor.SetSV(targetColor.GetSaturation() * damp, targetColor.GetValue() * damp);
            _icon.color = _icon.color.Lerp(targetColor, lerpSpeed * Time.deltaTime);
            float targetScale = _simpleButton.IsSelected ? 1f : 0.9f;
            targetScale *= _simpleButton.IsHovered ? 1 + (0.1f * hoverMult) : 1f;
            targetScale *= scaleMult.Remap(1f, 2f, 1f, 1.5f);
            _icon.transform.localScale = _icon.transform.localScale.Lerp(Vector3.one * targetScale, lerpSpeed * Time.deltaTime);

            Vector3 targetP = _icon.transform.localPosition;
            targetP.y = 0f;
            float rise = 4f;
            targetP.y += _submitPulseLerp.RemapPercent(0f, 2f * rise, false);
            targetP.y += _simpleButton.IsSelected ? rise : 0f;
            targetP.y += _simpleButton.IsHovered ? hoverMult * rise : 0f;
            _icon.transform.localPosition = _icon.transform.localPosition.Lerp(targetP, lerpSpeed * Time.deltaTime);
        }

        // Scale
        {
            float targetScale = 1f;
            targetScale *= _simpleButton.IsSelected ? 1.25f : 0.9f;
            targetScale *= _simpleButton.IsHovered ? 1 + (0.125f * hoverMult) : 1f;
            targetScale *= scaleMult;
            transform.localScale = transform.localScale.Lerp(Vector3.one * targetScale, lerpSpeed * Time.deltaTime);
        }

        // Cost
        {
            float lerp = 1.5f * lerpSpeed;
            float targetAlpha = (_simpleButton.IsSelected ? 1f : (_simpleButton.IsHovered? 0.75f : 0f));
            if (isUnlocked) targetAlpha = 0f;
            _costCGroup.alpha = _costCGroup.alpha.Lerp(targetAlpha, lerp * Time.deltaTime);
            _costText.text = $"-{NodeData.GemCost}";

            float targetScale = _simpleButton.IsSelected ? 1f : (_simpleButton.IsHovered ? 0.75f : 0.5f);
            targetScale *= scaleMult;
            if (isUnlocked) targetScale = 1.5f;
            if (!NodeData.CanAfford()) targetScale *= 0.75f;
            _costText.transform.parent.localScale = _costText.transform.parent.localScale.Lerp(Vector3.one * targetScale, lerp * Time.deltaTime);

            Color targetColor = (NodeData.CanAfford() || isUnlocked) ? Color.white : CantAffordColor.Lerp(Color.white, 0.125f);
            _costText.color = _costText.color.Lerp(targetColor, lerpSpeed * Time.deltaTime);
        }

        // Frame
        {
            _frame.color = _frame.color.Lerp(GetFrameColor(), lerpSpeed * Time.deltaTime);

            float targetScale = _simpleButton.IsSelected? 1.05f : 1f;
            targetScale *= _simpleButton.IsHovered ? 1 + (0.05f * hoverMult) : 1f;
            //targetScale *= scaleMult;
            targetScale = targetScale.Remap(1f, 2f, 1f, 1.25f);
            _frame.transform.localScale = _frame.transform.localScale.Lerp(Vector3.one * targetScale, lerpSpeed * Time.deltaTime);
        }

        // BG
        {
            Color targetColor = _baseBgColor;
            float lerp = 0;
            if (_simpleButton.IsSelected) lerp += 0.1f;
            if (_simpleButton.IsHovered) lerp += 0.1f * hoverMult;
            lerp += _submitPulseLerp.RemapPercent(0f, 0.05f);
            targetColor = targetColor.Lerp(Color.white, lerp);
            targetColor = targetColor.Lerp(_mainColor, 0.125f);
            _bg.color = _bg.color.Lerp(targetColor, lerpSpeed * Time.deltaTime);
        }
    }

    Color GetFrameColor()
    {
        Color targetColor = Color.magenta;
        if (IsUnlocked())
        {
            targetColor = UnlockedColor;
        }
        else
        {
            targetColor = NodeData.CanAfford() ? Color.white.Lerp(_mainColor, 0.25f) : CantAffordColor;
        }
        return targetColor;
    }

    void UpdateLines()
    {
        float zOffset = -50f;

        List<LineRenderer> lineRends = new(GetComponentsInChildren<LineRenderer>(true));
        var childNodes = Children;
        childNodes.RemoveAll(x => x == null);

        for (int childIndex = 0; childIndex < childNodes.Count; childIndex++)
        {
            var childNode = childNodes[childIndex];
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

            lineRend.enabled = true;
            lineRend.widthMultiplier = 0.15f;
            lineRend.material = Canvas.GetDefaultCanvasMaterial();
            lineRend.positionCount = 2;
            lineRend.SetPosition(0, ((RectTransform)transform).position + Vector3.forward * zOffset);
            lineRend.SetPosition(1, ((RectTransform)childNode.transform).position + Vector3.forward * zOffset);
            lineRend.startColor = Color.white.Lerp(_mainColor, 0.25f);// Color.white.Lerp(_mainColor, 0.25f);
            lineRend.endColor = Color.white.Lerp(childNode._mainColor, 0.25f); ;// Color.white.Lerp(childNode._mainColor, 0.25f);

            lineRend.gameObject.SetActive(childNode.IsRevealed() || !Application.isPlaying);
        }

        // Remove excess lines
        while (lineRends.Count > childNodes.Count)
        {
            Destroy(lineRends[childNodes.Count].gameObject);
            DestroyImmediate(lineRends[childNodes.Count].gameObject);
            lineRends.RemoveAt(childNodes.Count);
        }
    }
    #endregion
}
