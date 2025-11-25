using HyperQuest.EasyPooling;
using UnityEngine;

public class SIE_S_Telegraph : SIE, IPoolable
{
    float _telegraphPercent;

    float _cooldownTime => _skillInstance.Skill.Stats.Cooldown;
    float _size => _skillInstance.Skill.Stats.Size;
    float _telegraphTime => (0.2f + _cooldownTime * 0.2f);

    public void Initialize()
    {
        _telegraphPercent = 0f;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;

        // Tick
        _telegraphPercent += Time.deltaTime / _telegraphTime;

        // Scale
        transform.localScale = Vector3.one * _telegraphPercent.RemapPercent(0f, _size);

        // Next
        if (_telegraphPercent >= 1) _skillInstance.State++;
    }
}
