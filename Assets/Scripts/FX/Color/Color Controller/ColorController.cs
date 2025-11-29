using HyperQuest.EasyPooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public Color Color { get; private set; }
    public Action<Color> OnColorChanged;
    public Action<float> OnAlphaChanged;

    public void SetAlpha(float newAlpha)
    {
        Color = Color.Alpha(newAlpha);
        OnAlphaChanged?.Invoke(newAlpha);
    }

    public void SetColor(Color newColor)
    {
        Color = newColor;
        OnColorChanged?.Invoke(newColor);
        OnAlphaChanged?.Invoke(newColor.a);
    }
}
