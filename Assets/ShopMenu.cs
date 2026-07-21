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
    [SerializeField] SimpleButton _buyButton;
    [SerializeField] SimpleButton _nextButton;

    UpgradeManager _upgradeManager;

    List<SimpleButton> _buttonList = new();

    private void Start()
    {
        _upgradeManager = UpgradeManager.I;
        _buttonList.AddRange(_shopButtonHolder.GetComponentsInChildren<SimpleButton>());
        _buttonList.ForEach(button =>
        {
            button.OnDownEvent.AddListener(() =>
            {
                LoadButtonItemInfo(button);
            });

            if(button.IsHighlighted)
            {
                LoadButtonItemInfo(button);
            }
        });
        LoadInventory();
        UpdateButtonUI();

        Player.PlayerData.OnGemChange += () =>
        {
            LoadButtonItemInfo(GetHighlightedButton());
        };

        GetComponent<SimpleMenu>().OnOpenChanged += (open) =>
        {
            if(open)
            {
                var highlightedButton = GetHighlightedButton();
                LoadButtonItemInfo(highlightedButton);
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
    }

    SimpleButton GetHighlightedButton()
    {
        foreach(var button in _buttonList)
        {
            if (button.IsHighlighted) return button;
        }
        return null;
    }

    void LoadButtonItemInfo(SimpleButton button)
    {
        var item = GetButtonItem(button);

        if(_buyButton != null) _buyButton.SetInteractable(item != null && Player.PlayerData.Gems >= item.GemCost);

        if (button == null || item == null)
        {
            _descriptionText.text = string.Empty;
            return;
        }

        var newSkillUpgrade = new NewSkillUpgrade(item, null);
        _descriptionText.text = item.Name;
        _descriptionText.text += "\n" + newSkillUpgrade.GetDescription();
    }

    Skill GetButtonItem(SimpleButton button)
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

    NewSkillUpgrade GetButtonSkillUpgrade(SimpleButton button)
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

    void GenerateInventory()
    {
        var itemList = _upgradeManager.GetSkillList().FindAll(skill => skill.Slot == Skill.SlotEnum.Item  && Player.PlayerData.GetUnlockedSkillList().Contains(skill.Name));
        WeightedList<Skill> weightedItemList = new();
        itemList.ForEach(item => weightedItemList.Add(item, Constants.RarityToWeight(item.Rarity)));
        int itemCount = Random.Range(2, 4);
        for (int i = 0; i < itemCount; i++)
        {
            Inventory.Add(weightedItemList.SelectItem());
        }
        SaveInventory();
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
        Debug.Log("Shop Updating Button UI");
        //var itemList = _upgradeManager.GetSkillList().FindAll(skill => skill.Slot == Skill.SlotEnum.Item);
        for (int i = 0; i < _buttonList.Count; i++)
        {
            Debug.Log($"{i} < {Inventory.Count}");
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

        LoadButtonItemInfo(GetHighlightedButton());
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
