using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public List<Skill> Inventory = new();

    [SerializeField] int _gemSellCount = 2;
    [SerializeField] int _gemGoldValue = 2;
    [SerializeField] Transform _shopButtonHolder;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] SimpleButton_OldUI _buyButton;
    [SerializeField] SimpleButton_OldUI _nextButton;
    [SerializeField] TextMeshProUGUI _costText;

    UpgradeManager _upgradeManager;

    List<SimpleButton_OldUI> _buttonList = new();

    private void Start()
    {
        _upgradeManager = UpgradeManager.I;
        _buttonList.AddRange(_shopButtonHolder.GetComponentsInChildren<SimpleButton_OldUI>());
        _buttonList.ForEach(button =>
        {
            button.OnDownEvent.AddListener(() =>
            {
                LoadInfoPannelWithButton(button);
            });

            if(button.IsHighlighted)
            {
                LoadInfoPannelWithButton(button);
            }
        });
        LoadInventory();
        UpdateButtonUI();

        Player.PlayerData.OnGemChange += (delta) =>
        {
            LoadInfoPannelWithButton(GetHighlightedButton());
        };

        GetComponent<SimpleMenu>().OnOpenChanged += (open) =>
        {
            if(open)
            {
                var highlightedButton = GetHighlightedButton();
                LoadInfoPannelWithButton(highlightedButton);
            }
        };

        _buyButton.OnSubmitEvent.AddListener(() => 
        {
            BuySelectedItem();
        });
    }

    public void BuySelectedItem()
    {
        var itemUpgrade = GetHighlightedSkillUpgrade();

        // TEMPORARY - Need to add choice of which player to add item to on co-op
        itemUpgrade.ApplyUpgrade(PlayerManager.I.PlayerList[0].Actor);
        Player.PlayerData.Gems -= itemUpgrade.SourceSkill.GemCost;
        var highlightedButton = GetHighlightedButton();
        highlightedButton.IsHighlighted = false;
        Inventory.Remove(GetButtonItem(highlightedButton));

        // Highlight first item in shop if available
        for(int i = 0; i < _buttonList.Count; i++)
        {
            var item = GetButtonItem(_buttonList[i]);
            if (item == null) continue;

            _buttonList[i].IsHighlighted = true;
            break;
        }

        // If can no longer buy, select highlighted item in shop or next
        if(!_buyButton.GetInteractable())
        {
            var selectedButton = GetHighlightedButton();
            if (selectedButton == null) selectedButton = _nextButton;
            EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
        }

        UpdateButtonUI();

        SaveInventory();
    }

    SimpleButton_OldUI GetHighlightedButton()
    {
        foreach(var button in _buttonList)
        {
            if (button.IsHighlighted) return button;
        }
        return null;
    }

    void LoadInfoPannelWithButton(SimpleButton_OldUI button)
    {
        var item = GetButtonItem(button);

        /*
        bool itemSlotsRemain = false;
        foreach(var player in PlayerManager.I.PlayerList)
        {
            if (player.Actor.SkillSystem.ItemList.Count < Player.CampUpgradeData.ItemSlotCount)
            {
                itemSlotsRemain = true;
                break;
            }
        }
        */

        //if(_buyButton != null) _buyButton.SetInteractable(item != null && Player.PlayerData.Gems >= item.GemCost && itemSlotsRemain);

        if (button == null || item == null)
        {
            if(_descriptionText != null) _descriptionText.text = string.Empty;
            if(_costText != null) _costText.transform.parent.gameObject.SetActive(false);
            return;
        }
        _costText.transform.parent.gameObject.SetActive(true);

        var newSkillUpgrade = new NewSkillUpgrade(item, null);
        _descriptionText.text = item.Name;
        _descriptionText.text += "\n" + newSkillUpgrade.GetDescription();

        _costText.text = "-" + item.GemCost.ToString();
    }

    Skill GetButtonItem(SimpleButton_OldUI button)
    {
        if (button == null) return null;
        int index = _buttonList.IndexOf(button);
        if (index >= Inventory.Count) return null;
        return Inventory[index];
    }

    NewSkillUpgrade GetHighlightedSkillUpgrade()
    {
        return GetButtonSkillUpgrade(GetHighlightedButton());
    }

    NewSkillUpgrade GetButtonSkillUpgrade(SimpleButton_OldUI button)
    {
        int index = _buttonList.IndexOf(button);
        if (index >= Inventory.Count) return null;
        var item = Inventory[index];
        return new NewSkillUpgrade(item, null);
    }

    void LoadInventory()
    {
        Inventory.Clear();
        var inventoryString = PlayerPrefs.GetString("ShopInventory", string.Empty);

        if(inventoryString == string.Empty)
        {
            GenerateInventory();
            return;
        }

        string[] inventoryNames = inventoryString.Split(",");
        for(int i = 0; i < inventoryNames.Length; i++)
        {
            if (inventoryNames[i] == string.Empty || inventoryNames[i] == " ")
            {
                Inventory.Add(null);
                continue;
            }

            var loadedSkill = _upgradeManager.GetSkillList().Find(skill => skill.Slot == Skill.SlotEnum.Item && skill.Name == inventoryNames[i]);
            Inventory.Add(loadedSkill);
        }
    }

    public void GenerateInventory(int count = -1)
    {
        var weightedItemList = GetWeightedItemList();
        if(count < 0) count = Random.Range(2, 4);
        for (int i = 0; i < count; i++)
        {
            Inventory.Add(weightedItemList.SelectItem());
        }
        SaveInventory();
        UpdateButtonUI();
    }

    WeightedList<Skill> GetWeightedItemList()
    {
        var unlockedSkillList = Player.PlayerData.GetUnlockedSkillList();
        var itemList = _upgradeManager.GetSkillList().FindAll(skill => skill.Slot == Skill.SlotEnum.Item && (unlockedSkillList.Contains("all") || unlockedSkillList.Contains(skill.Name)));
        WeightedList<Skill> weightedItemList = new();
        itemList.ForEach(item => weightedItemList.Add(item, Constants.RarityToWeight(item.Rarity)));
        return weightedItemList;
    }

    public void RerollShop()
    {
        int cost = 4;
        if (Player.PlayerData.Gems < cost) return;
        Player.PlayerData.Gems -= cost;

        Inventory.Clear();
        GenerateInventory();
    }

    void SaveInventory()
    {
        string inventoryString = "";
        for(int i = 0; i < Inventory.Count; i++)
        {
            inventoryString += Inventory[i].name;
            if (i < Inventory.Count - 1) inventoryString += ",";
        }
        PlayerPrefs.SetString("ShopInventory", inventoryString);
    }

    void UpdateButtonUI()
    {
        //Debug.Log("Shop Updating Button UI");
        //var itemList = _upgradeManager.GetSkillList().FindAll(skill => skill.Slot == Skill.SlotEnum.Item);
        for (int i = 0; i < _buttonList.Count; i++)
        {
            //Debug.Log($"{i} < {Inventory.Count}");
            if (i < Inventory.Count)
            {
                _buttonList[i].Sprite = Inventory[i].Icon;
                _buttonList[i].MainColor = Inventory[i].GetColor();
                _buttonList[i].SetInteractable(true);
            }
            else
            {
                
                _buttonList[i].Sprite = null;
                _buttonList[i].MainColor = Color.clear;
                _buttonList[i].SetInteractable(false);
            }
        }

        LoadInfoPannelWithButton(GetHighlightedButton());
    }

    public void SellGems()
    {
        // Can Players afford?
        if (Player.PlayerData.Gems < _gemSellCount) return;

        // Gem Cost
        Player.PlayerData.Gems -= _gemSellCount;

        // Add gold randomly to party
        for(int gold = 0; gold < _gemGoldValue; gold++)
        {
            PlayerManager.I.PlayerList.GetRandomElement().Data.Gold++;
        }
    }
}
