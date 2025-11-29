using HyperQuest.EasyPooling;
using UnityEngine;

public class SIE_A_Duration : SIE, IPoolable
{
    public float LifePercent { get; private set; }
    ColorController _colorController;

    protected override void Awake()
    {
        base.Awake();
        _colorController = GetComponentInParent<ColorController>();
    }

    public void Initialize()
    {
        LifePercent = 0f;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;

        _colorController.SetAlpha(_colorController.Color.a.Lerp(Constants.SkillStats.BaseAlpha, 24f * Time.deltaTime));
    }

    private void FixedUpdate()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;

        LifePercent += Time.fixedDeltaTime / _skillInstance.Skill.Stats.Duration;
        LifePercent = Mathf.Clamp01(LifePercent);

        if (LifePercent >= 1) _skillInstance.State++;
    }
}
