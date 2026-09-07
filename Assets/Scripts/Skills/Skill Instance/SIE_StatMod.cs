using ManaSprite.EasyPooling;
using System.Collections.Generic;
using UnityEngine;

public class SIE_StatMod : SIE, IPoolable
{
    [SerializeField] SpriteRenderer _icon;
    public List<UpgradeMod> EffectMods;

    private void OnDestroy()
    {
        UpgradeMod.RemoveModList(EffectMods, _skillInstance);
    }

    public void Initialize()
    {
        transform.SetParent(_skillInstance.Skill.Actor.transform);
        transform.localPosition = Vector3.zero;
        _icon.transform.localPosition = Vector3.zero + Vector3.forward * _icon.transform.localPosition.z;
        _icon.color = _icon.color.Alpha(0f);
    }

    private void Update()
    {
        if (_skillInstance.StartPercent < 1f)
        {
            _skillInstance.StartPercent += Time.deltaTime / 1f;
            _icon.transform.position += 1f * Vector3.up * Time.deltaTime;
            float alphaTarget = _skillInstance.StartPercent < 0.5f? 1f : 0f;
            _icon.color = _icon.color.Alpha(_icon.color.a.Lerp(alphaTarget, 12f * Time.deltaTime));
        }
        else if(_skillInstance.ActivePercent < 1f)
        {
            float duration = _skillInstance.Skill.Stats.Duration.Value;
            _skillInstance.ActivePercent += duration <= 0 ? 1f : (Time.deltaTime / duration);
            
            if(_skillInstance.State != SkillInstance.SkillInstanceState.Activated)
            {
                UpgradeMod.ApplyModList(EffectMods, _skillInstance);
                _skillInstance.State = SkillInstance.SkillInstanceState.Activated;
            }            
        }
        else if (_skillInstance.EndPercent < 1f)
        {
            if (_skillInstance.State != SkillInstance.SkillInstanceState.End)
            {
                Debug.Log("SIE_StatMod END");
                UpgradeMod.RemoveModList(EffectMods, _skillInstance);
                _skillInstance.State = SkillInstance.SkillInstanceState.End;
            }
        }
    }
}
