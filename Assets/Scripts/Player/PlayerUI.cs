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

    private void Start()
    {
        LoadData();
        this.DelayedInvoke(-1, () =>
        {
            LoadData();
        });
    }

    public void SetPlayer(Player player)
    {
        if (this.Player != null) Player.Data.OnCoinChange -= LoadCoins;
        if (this.Player != null) Player.Actor.OnUpgrade -= LoadData;
        if (this.Player != null) Player.CampUpgradeData.OnCampDataChange -= LoadData;
        this.Player = player;
        if (this.Player != null) Player.Data.OnCoinChange += LoadCoins;
        if (this.Player != null) Player.Actor.OnUpgrade += LoadData;
        if (this.Player != null) Player.Actor.SkillSystem.OnSkillAdded += (skill, actor) => LoadData();
        if (this.Player != null) Player.CampUpgradeData.OnCampDataChange += LoadData;
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
        HandleSlots(_activeSlots, Player.CampUpgradeData.MainSlotCount, skillSystem.ActiveSkillList);
        HandleSlots(_passiveSlots, Player.CampUpgradeData.PassiveSlotCount, skillSystem.PassiveSkillList);
        //HandleSlots(_itemSlots, Player.CampUpgradeData.ItemSlotCount, skillSystem.ItemList);
    }

    void HandleSlots(List<SkillSlotUI> slotList, int slotCount = 1, List<Skill> skillList = null) 
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].gameObject.SetActive(i < slotCount);
            if (i < slotCount)
            {
                if (skillList != null)
                {
                    slotList[i].SetSkill(i < skillList.Count ? skillList[i] : null);
                }
                else
                {
                    slotList[i].SetSkill(null);
                }
            }
        }
    }
}
