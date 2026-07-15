using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image _avatar;
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] List<SkillSlotUI> _activeSlots;
    [SerializeField] List<SkillSlotUI> _passiveSlots;
    [SerializeField] List<SkillSlotUI> _itemSlots;

    public Player Player { get; private set; }

    public void SetPlayer(Player player)
    {
        if (this.Player != null) Player.Data.OnCoinChange -= LoadCoins;
        if (this.Player != null) Player.Actor.OnUpgrade -= LoadData;
        this.Player = player;
        if (this.Player != null) Player.Data.OnCoinChange += LoadCoins;
        if (this.Player != null) Player.Actor.OnUpgrade += LoadData;
        LoadData();
    }

    void LoadCoins()
    {
        _goldText.text = (Player?.Data.Gold ?? 0).ToString();
    }

    void LoadData()
    {
        // Avatar
        _avatar.sprite = Player?.Actor.Sprite;
        _avatar.color = Player?.Data.Color ?? Color.black;

        LoadCoins();

        // Skills
        var skillSystem = Player?.Actor?.SkillSystem;
        for(int i = 0; i < _activeSlots.Count; i++)
        {
            if (skillSystem != null)
            {
                _activeSlots[i].SetSkill(i < skillSystem.ActiveSkillList.Count ? skillSystem.ActiveSkillList[i] : null);
                _passiveSlots[i].SetSkill(i < skillSystem.PassiveSkillList.Count ? skillSystem.PassiveSkillList[i] : null);
                _itemSlots[i].SetSkill(i < skillSystem.ItemList.Count ? skillSystem.ItemList[i] : null);
            }
            else
            {
                _activeSlots[i].SetSkill(null);
                _passiveSlots[i].SetSkill(null);
                _itemSlots[i].SetSkill(null);
            }
        }
    }
}
