using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CampMenu : MonoBehaviour
{
    [SerializeField] SimpleButton _skillSlotBtn;
    [SerializeField] SimpleButton _passiveSlotBtn;
    [SerializeField] SimpleButton _itemSlotBtn;
    [SerializeField] SimpleButton _lootBtn;
    [SerializeField] SimpleButton _hordeBtn;
    List<SimpleButton> _buttonList = new();

    [SerializeField] SimpleButton _buyButton;

    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] TextMeshProUGUI _cost;

    private void Awake()
    {
        _buttonList = new();
        _buttonList.AddRange(new SimpleButton[] 
        { 
            _skillSlotBtn, 
            _passiveSlotBtn,
            _itemSlotBtn,
            _lootBtn,
            _hordeBtn 
        });
    }

    private void Start()
    {
        _buttonList.ForEach(button =>
        {
            button.OnDownEvent.AddListener(() =>
            {
                LoadInfoPannelForButton(button);
            });

            if(button.IsHighlighted)
            {
                LoadInfoPannelForButton(button);
            }
        });

        _buyButton.OnSubmitEvent.AddListener(() =>
        {
            BuySelectedUpgrade();
        });

        Player.PlayerData.OnGemChange += (delta) =>
        {
            UpdateUI();
        };

        UpdateUI();
    }

    SimpleButton GetHighlightedButton()
    {
        foreach (var button in _buttonList)
        {
            if (button.IsHighlighted) return button;
        }
        return null;
    }

    void BuySelectedUpgrade()
    {
        var button = GetHighlightedButton();
        int cost = GetCostForButtonUpgrade(button);
        Player.PlayerData.Gems -= cost;

        /*
        if (button == _skillSlotBtn)
        {
            Player.CampUpgradeData.SkillSlotLevel++;
        }
        else if (button == _passiveSlotBtn)
        {
            Player.CampUpgradeData.PassiveSlotUpgradeCount++;
        }
        else if (button == _itemSlotBtn)
        {
            Player.CampUpgradeData.ItemSlotUpgradeCount++;   
        }
        else if (button == _lootBtn)
        {
            Player.CampUpgradeData.LootLevel++;
        }
        else if (button == _hordeBtn)
        {
            Player.CampUpgradeData.HordeLevel++;
        }
        */

        UpdateUI();
    }

    int GetLevelOfButton(SimpleButton button)
    {
        /*
        if (button == _skillSlotBtn)
        {
            return Player.CampUpgradeData.SkillSlotLevel;
        }
        else if (button == _passiveSlotBtn)
        {
            return Player.CampUpgradeData.PassiveSlotUpgradeCount;
        }
        else if (button == _itemSlotBtn)
        {
            return Player.CampUpgradeData.ItemSlotUpgradeCount;
        }
        else if (button == _lootBtn)
        {
            return Player.CampUpgradeData.LootLevel;
        }
        else if (button == _hordeBtn)
        {
            return Player.CampUpgradeData.HordeLevel;
        }
        */
        return 0;
    }

    int GetCostForButtonUpgrade(SimpleButton button)
    {
        /*
        if (button == _skillSlotBtn)
        {
            return (int)Mathf.Pow(2, 4 + Player.CampUpgradeData.SkillSlotLevel);
        }
        else if (button == _passiveSlotBtn)
        {
            return (int)Mathf.Pow(2, 4 + Player.CampUpgradeData.PassiveSlotUpgradeCount);
        }
        else if (button == _itemSlotBtn)
        {
            return (int)Mathf.Pow(2, 4 + Player.CampUpgradeData.ItemSlotUpgradeCount);
        }
        else if (button == _lootBtn)
        {
            return (int)Mathf.Pow(2, 4 + Player.CampUpgradeData.LootLevel);
        }
        else if(button == _hordeBtn)
        {
            return (int)Mathf.Pow(2, 4 + Player.CampUpgradeData.HordeLevel);
        }
        */
        return 0;
    }

    void LoadInfoPannelForButton(SimpleButton button)
    {
        int buttonLevel = GetLevelOfButton(button);

        if(button == null)
        {
            _description.text = string.Empty;
        }
        else if(button == _skillSlotBtn)
        {
            _description.text = "SKILL SLOT";
            if(buttonLevel < 3) _description.text += "\n+1 main/offhand slot.";
        }
        else if(button == _passiveSlotBtn)
        {
            _description.text = "PASSIVE SLOT";
            if (buttonLevel < 3) _description.text += "\n+1 passive slot.";
        }
        else if(button == _itemSlotBtn)
        {
            _description.text = "ITEM SLOT";
            if (buttonLevel < 3) _description.text += "\n+1 item slot.";
        }
        else if(button == _lootBtn)
        {
            _description.text = "LOOTING";
            if (buttonLevel < 3) _description.text += "\nLoot Multiplier:".Color(Constants.Colors.LabelColorHex) + $"\n+{12 * (Player.CampUpgradeData.LootLevel)}%" + (Player.CampUpgradeData.LootLevel >= 3? string.Empty : (" -> " + $"+{ 12 * (Player.CampUpgradeData.LootLevel + 1)}%".Color(Constants.Colors.PositiveStatColor)));
        }
        else if(button == _hordeBtn)
        {
            _description.text = "HORDE";
            if (buttonLevel < 3) _description.text += $"\nStarting Gold:\n".Color(Constants.Colors.LabelColorHex) + $"{20 * (Player.CampUpgradeData.HordeLevel)}" + (Player.CampUpgradeData.HordeLevel >= 3? string.Empty : (" -> " + $"{20 * (1 + Player.CampUpgradeData.HordeLevel)}".Color(Constants.Colors.PositiveStatColor)));
        }

        int cost = GetCostForButtonUpgrade(button);
        _cost.text = "-" + cost.ToString();
        if(_cost != null) _cost.transform.parent.gameObject.SetActive(cost > 0);

        if (_buyButton != null)
        {
            _buyButton.SetInteractable(Player.PlayerData.Gems >= cost && buttonLevel < 3);
            _buyButton.gameObject.SetActive(buttonLevel < 3);
        }
    }

    void UpdateUI()
    {
        /*
        SetButtonLevel(_skillSlotBtn, Player.CampUpgradeData.SkillSlotLevel);
        SetButtonLevel(_passiveSlotBtn, Player.CampUpgradeData.PassiveSlotUpgradeCount);
        SetButtonLevel(_itemSlotBtn, Player.CampUpgradeData.ItemSlotUpgradeCount);
        SetButtonLevel(_lootBtn, Player.CampUpgradeData.LootLevel);
        SetButtonLevel(_hordeBtn, Player.CampUpgradeData.HordeLevel);
        LoadInfoPannelForButton(GetHighlightedButton());
        */
    }

    void SetButtonLevel(SimpleButton button, int level)
    {
        if (button == null) return;
        for(int i = 0; i <  button.LevelPipHolder.childCount; i++)
        {
            bool isFilled = i < level;
            var child = button.LevelPipHolder.GetChild(i);
            child.GetChild(child.childCount - 1).gameObject.SetActive(isFilled);
        }
    }
}
