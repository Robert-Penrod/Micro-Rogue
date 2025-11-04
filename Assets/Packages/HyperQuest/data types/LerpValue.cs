using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LerpValue
{
    public float Value;
    public float Target;
    public float LerpMult = 1f;

    public LerpValue(float value, float target, float lerpMult)
    {
        Value = value;
        Target = target;
        LerpMult = lerpMult;
    }

    public void Lerp(float deltaTime)
    {
        Value = Mathf.Lerp(Value, Target, LerpMult * deltaTime);
    }

    public void SetPercentage(float percentage)
    {
        Value = Mathf.Lerp(Value, Target, percentage);
    }

    public void SetValue(float value)
    {
        Value = value;
        Target = value;
    }
}
