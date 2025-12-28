using ManaSprite.EasyPooling;
using UnityEngine;

public class SIE_A_Duration : SIE, IPoolable
{
    ColorController _colorController;

    protected override void Awake()
    {
        base.Awake();
        _colorController = GetComponentInParent<ColorController>();
    }

    public void Initialize()
    {
        _skillInstance.ActivePercent = 0f;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;

        _colorController.SetAlpha(_colorController.Color.a.Lerp(Constants.SkillStats.BaseAlpha, 24f * Time.deltaTime));
    }

    private void FixedUpdate()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;

        _skillInstance.ActivePercent += Time.fixedDeltaTime / _skillInstance.Skill.Stats.Duration.Value;
        _skillInstance.ActivePercent = Mathf.Clamp01(_skillInstance.ActivePercent);

        if (_skillInstance.ActivePercent >= 1) _skillInstance.State++;
    }
}
