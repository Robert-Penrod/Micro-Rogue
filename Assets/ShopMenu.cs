using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] int _gemSellCount = 2;
    [SerializeField] int _gemGoldValue = 2;
    [SerializeField] Transform _shopButtonHolder;

    UpgradeManager _upgradeManager;

    List<SimpleButton> _buttonList = new();

    private void Start()
    {
        _upgradeManager = UpgradeManager.I;
        _buttonList.AddRange(_shopButtonHolder.GetComponentsInChildren<SimpleButton>());
        var itemList = _upgradeManager.GetSkillList().FindAll(skill => skill.Slot == Skill.SlotEnum.Item);
        for (int i = 0; i < _buttonList.Count; i++)
        {
            if(i < itemList.Count)
            {
                _buttonList[i].Sprite = itemList[i].Icon;
                _buttonList[i].MainColor = itemList[i].GetColor();
                _buttonList[i].RefreshUI();
            }
            else
            {
                _buttonList[i].gameObject.SetActive(false);
            }
        }
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
