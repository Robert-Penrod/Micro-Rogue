using HyperQuest.EasyPooling;
using UnityEngine;

public class SIE_A_Duration : SIE, IPoolable
{
    float _lifePercent;

    public void Initialize()
    {
        _lifePercent = 0f;
    }

    private void FixedUpdate()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated) return;

        _lifePercent += Time.fixedDeltaTime / _skillInstance.Skill.Stats.Duration;
        _lifePercent = Mathf.Clamp01(_lifePercent);

        if (_lifePercent >= 1) _skillInstance.State++;
    }
}
