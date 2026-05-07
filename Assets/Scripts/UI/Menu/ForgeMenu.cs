using System.Collections.Generic;
using UnityEngine;

public class ForgeMenu : MonoBehaviour
{
    [SerializeField] Transform _skillGrid;
    List<UnlockSlot> _unlockSlots = new();

    CanvasGroup _canvasGroup;
    bool _isOpen;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(_isOpen ? 1f : 0f, 12f * Time.deltaTime);
    }

    public void SetOpen(bool isOpen)
    {
        _isOpen = isOpen;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = _isOpen;
        CameraManager.I.Zoom(isOpen? 0.5f : 1f, this);
    }

    void Init()
    {
        var unlockedSkills = Player.PlayerData.GetUnlockedSkillList();
        _unlockSlots.Clear();
        for (int i = 0; i < _skillGrid.childCount; i++)
        {
            var unlockSlot = _skillGrid.GetChild(i).GetComponent<UnlockSlot>();
            _unlockSlots.Add(unlockSlot);
        }
        var skillList = UpgradeManager.I.GetSkillList();
        for(int i = 0; i < _unlockSlots.Count; i++)
        {
            if (i < skillList.Count)
            {
                bool isUnlocked = unlockedSkills.Contains(skillList[i].name);

                _unlockSlots[i].Icon.enabled = true;
                _unlockSlots[i].Icon.sprite = skillList[i].Icon;
                _unlockSlots[i].Icon.color = isUnlocked? skillList[i].GetColor() : Color.black;
                _unlockSlots[i].BG.color = isUnlocked ? Color.grey : new Color(0.15f, 0.15f, 0.15f);
            }
            else
            {
                _unlockSlots[i].Icon.enabled = false;
            }
        }

        SetOpen(false);
    }
}
