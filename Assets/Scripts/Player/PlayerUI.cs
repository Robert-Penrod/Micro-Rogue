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
        _goldText.text = (Player?.Data.Coin ?? 0).ToString();
    }

    void LoadData()
    {
        // Avatar
        _avatar.sprite = Player?.Actor.Sprite;
        _avatar.color = Player?.Data.Color ?? Color.black;

        LoadCoins();

        // Skills
        var skillSystem = Player.Actor.SkillSystem;
        for(int i = 0; i < 2; i++)
        {
            _activeSlots[i].SetSkill(i < skillSystem.ActiveSkillList.Count? skillSystem.ActiveSkillList[i] : null);
            _passiveSlots[i].SetSkill(i < skillSystem.PassiveSkillList.Count ? skillSystem.PassiveSkillList[i] : null);
        }
    }
}
