using HyperQuest.EasyPooling;
using System;
using UnityEngine;

public class SkillInstance : MonoBehaviour, IPoolable
{
    [HideInInspector] public Skill Skill;
    [SerializeField] AudioClip _startClip;
    [SerializeField] AudioClip _activeClip;
    [SerializeField] AudioClip _endClip;

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
    }

    void PlayAudio(AudioClip audio, float volMult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(audio, 0.2f, 1f, 0.25f * volMult, transform.position);
    }
}
