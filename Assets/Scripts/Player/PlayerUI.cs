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

    private void Awake()
    {
        PlayerManager.I.OnPlayerJoin += (Player player) =>
        {
            this.DelayedInvoke(-1, () =>
            {
                SetPlayer(player);
            });
        };
    }

    public void SetPlayer(Player player)
    {
        this.Player = player;
        Player.Actor.OnUpgrade += () =>
        {
            LoadData();
        };
        LoadData();
    }

    void LoadData()
    {
        // Avatar
        _avatar.sprite = Player.Actor.Sprite;
        _avatar.color = Player.Data.Color;

        // Skills
        var skillSystem = Player.Actor.SkillSystem;
        for(int i = 0; i < 2; i++)
        {
            _activeSlots[i].SetSkill(i < skillSystem.ActiveSkillList.Count? skillSystem.ActiveSkillList[i] : null);
            _passiveSlots[i].SetSkill(i < skillSystem.PassiveSkillList.Count ? skillSystem.PassiveSkillList[i] : null);
        }
    }
}
