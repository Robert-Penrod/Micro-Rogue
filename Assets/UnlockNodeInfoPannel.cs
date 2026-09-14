using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnlockNodeInfoPannel : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Image _bg;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] TextMeshProUGUI _typeText;

    private void Start()
    {
        EventSystemSingleton.I.OnSelectionChange += UpdateGFX;
    }

    private void OnDestroy()
    {
        EventSystemSingleton.I.OnSelectionChange -= UpdateGFX;
    }

    void UpdateGFX()
    {
        var selectedUnlockNode = EventSystem.current.currentSelectedGameObject == null ? null : EventSystem.current.currentSelectedGameObject.GetComponent<UnlockNodeButton>();
        transform.GetChild(0).gameObject.SetActive(selectedUnlockNode != null);
        if(selectedUnlockNode != null)
        {
            _bg.color = selectedUnlockNode._baseBgColor.Lerp(selectedUnlockNode._mainColor, 0.05f).Alpha(_bg.color.a);

            var nodeData = selectedUnlockNode.NodeData;
            if(nodeData.UnlockedStringInfo != null)
            {
                string unlockString = nodeData.UnlockedStringInfo.UnlockedString;
                _title.text = unlockString;
                _description.text = nodeData.UnlockedStringInfo.Description;
                _icon.sprite = nodeData.UnlockedStringInfo.Sprite;
                _icon.color = nodeData.UnlockedStringInfo.Color;
                _typeText.text = string.Empty;
                if (unlockString.Contains("Slot") || _description.text.ToLower().Contains("slot")) _typeText.text = "Slot";
                else _typeText.text = "Stat";
            }
            else if(nodeData.UnlockedSkill != null)
            {
                _title.text = nodeData.UnlockedSkill.Name;
                _description.text = (new NewSkillUpgrade(nodeData.UnlockedSkill, null)).GetDescription();
                _icon.sprite = nodeData.UnlockedSkill.Icon;
                _icon.color = nodeData.UnlockedSkill.GetColor().SetValue(1f);
                _typeText.text = "Skill";
            }
            else if (nodeData.UnlockedCharacter != null)
            {
                _title.text = nodeData.UnlockedCharacter.name;
                _description.text = string.Empty;
                _icon.sprite = nodeData.UnlockedCharacter.Sprite;
                _icon.color = nodeData.UnlockedCharacter.Color;
                _typeText.text = "Char";
            }
        }
    }
}
