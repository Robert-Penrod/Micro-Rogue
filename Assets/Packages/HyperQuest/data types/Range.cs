
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Range
{
    public float Min;
    public float Max;

    public Range()
    {

    }

    public Range(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float GetRandValue() => Random.Range(Min, Max);

    public float GetAverage() => 0.5f * (Max + Min);

    public Range Clone()
    {
        return new Range(Min, Max);
    }
}