using ManaSprite.EasyPooling;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillInstance : MonoBehaviour, IPoolable
{
    public Rigidbody2D ParentBody;

    public List<GameObject> _previousTargetsList = new();

    public Skill Skill;
    [SerializeField] AudioClip _startClip;
    [SerializeField] AudioClip _activeClip;
    [SerializeField] AudioClip _endClip;

    public float StartPercent;
    public float ActivePercent;
    public float EndPercent;

    public enum SkillInstanceState { Start, Activated, End}

    public SkillInstanceState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (value <= _state) return;
            int stateCount = Enum.GetNames(typeof(SkillInstanceState)).Length;
            if ((int)value > stateCount) _state = (SkillInstanceState)stateCount;
            else _state = value;
            switch (_state)
            {
                case SkillInstanceState.Activated:
                    OnActivated?.Invoke();
                    PlayAudio(_activeClip);
                    break;
                case SkillInstanceState.End:
                    OnEnd?.Invoke();
                    PlayAudio(_endClip);
                    if (Skill.SkillInstances.Contains(this)) Skill.SkillInstances.Remove(this);
                    break;
            }
        }
    }
    SkillInstanceState _state;

    public Action OnStart;
    public Action OnActivated;
    public Action OnEnd;

    public void Initialize()
    {
        _state = SkillInstanceState.Start;
        OnStart?.Invoke();
        PlayAudio(_startClip, 0.5f);
        StartPercent = ActivePercent = EndPercent = 0f;
    }

    public void Link(Skill skill)
    {
        this.Skill = skill;
        if(!skill.SkillInstances.Contains(this)) skill.SkillInstances.Add(this);
    }

    void PlayAudio(AudioClip audio, float volMult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(audio, 0.2f, 1f, 0.25f * volMult, transform.position);
    }
}
