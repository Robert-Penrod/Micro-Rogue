using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Image _cooldownFx;
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
        _cooldownFx.enabled = skill != null && skill.Stats.Rate.Value > 0;

        if(_skill != null)
        {
            _icon.sprite = _skill.Icon;
            var skillColor = _skill.SkillColor == Color.clear? GamePaletteManager.I.Palette.GetSkillColor(_skill).SetValue(1f) : _skill.SkillColor;
            _icon.color = Color.white.Lerp(skillColor, 1f).Alpha(_icon.color.a);
            _lvlText.text = $"Lvl {skill.Level}";
        }
    }

    private void Update()
    {
        if(_skill != null)
        {
            _cooldownFx.enabled = !_skill.IsActive && _skill.Stats.Rate.Value > 0;
            _cooldownFx.fillAmount = 1f - _skill.CooldownPercent;

            float targetSize = _skill.IsActive ? 1.25f : 1f;
            float lerpSize = _icon.transform.localScale.x.Lerp(targetSize, 12f * Time.deltaTime);
            _icon.transform.localScale = Vector3.one * lerpSize;
        }
    }
}
