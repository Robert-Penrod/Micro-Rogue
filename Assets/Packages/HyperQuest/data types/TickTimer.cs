using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Timer that must be manually "ticked" to pass time
/// </summary>
[System.Serializable]
public class TickTimer
{
    public float MaxTime;
    float tick;

    public TickTimer(float maxTime = 0f)
    {
        MaxTime = maxTime;
        tick = 0f;
    }

    public bool Tick(float deltaTime)
    {
        if (IsDone()) return true;
        tick += deltaTime;
        return false;
    }

    public void ResetByMaxTime()
    {
        tick -= MaxTime;
    }

    public void Reset()
    {
        tick = 0f;
    }

    public void Reset(float newTime)
    {
        MaxTime = newTime;
        Reset();
    }

    public bool IsDone()
    {
        return tick >= MaxTime;
    }

    public void SetFinished()
    {
        tick = MaxTime;
    }

    public float GetPercent() => Mathf.Clamp01(tick / MaxTime);
    public void SetPercent(float percent)
    {
        tick = percent * MaxTime;
    }
}
