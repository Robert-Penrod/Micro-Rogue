using HyperQuest.EasyPooling;
using System;
using UnityEngine;

public class SkillInstance : MonoBehaviour, IPoolable
{
    public Skill Skill;

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
                    OnStart?.Invoke();
                    break;
                case SkillInstanceState.End:
                    OnEnd?.Invoke();
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
    }
}
