using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _lvlText;

    Skill _skill;

    private void Awake()
    {
        SetSkill(null);
    }

    public void SetSkill(Skill skill)
    {
        this._skill = skill;

        _icon.enabled = _skill != null;
        _lvlText.enabled = skill != null;

        if(_skill != null)
        {
            _icon.sprite = _skill.Icon;
            var skillColor = GamePaletteManager.I.Palette.GetSkillColor(_skill);
            _icon.color = Color.white.Lerp(skillColor, 1f).Alpha(_icon.color.a);
            _lvlText.text = $"Lvl {skill.Level}";
        }
    }
}
