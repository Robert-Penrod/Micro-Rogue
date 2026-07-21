using System.Collections.Generic;
using UnityEngine;

public class ForgeMenu : MonoBehaviour
{
    [SerializeField] Transform _skillGrid;
    List<SimpleButton> _unlockSlots = new();


    private void Start()
    {
        Init();
    }

    void Init()
    {
        var unlockedSkills = Player.PlayerData.GetUnlockedSkillList();
        _unlockSlots.Clear();
        for (int i = 0; i < _skillGrid.childCount; i++)
        {
            var unlockSlot = _skillGrid.GetChild(i).GetComponent<SimpleButton>();
            _unlockSlots.Add(unlockSlot);
        }
        var skillList = UpgradeManager.I.GetSkillList();
        for(int i = 0; i < _unlockSlots.Count; i++)
        {
            if (i < skillList.Count)
            {
                bool isUnlocked = unlockedSkills.Contains(skillList[i].name);

                _unlockSlots[i].Sprite = skillList[i].Icon;
                _unlockSlots[i].MainColor = isUnlocked? skillList[i].GetColor() : Color.black;
                //_unlockSlots[i].BG.color = isUnlocked ? Color.grey : new Color(0.15f, 0.15f, 0.15f);
            }
            else
            {
                //_unlockSlots[i].Icon.enabled = false;
                _unlockSlots[i].Sprite = null;
            }
            _unlockSlots[i].RefreshUI();
        }
    }
}
