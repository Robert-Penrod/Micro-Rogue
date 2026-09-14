using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HueShifter : MonoBehaviour
{
    SpriteRenderer _sprite;
    Image _image;
    Color _startingColor;
    public float Speed;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _image = GetComponent<Image>();
        _startingColor = GetColor();
    }

    Color GetColor()
    {
        if (_sprite != null) return _sprite.color;
        if (_image != null) return _image.color;
        return Color.clear;
    }

    void SetColor(Color newColor)
    {
        if (_sprite != null) _sprite.color = newColor;
        if (_image != null) _image.color = newColor;
    }

    private void Update()
    {
        Color color = GetColor();
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h += Speed * Time.deltaTime;
        Color newColor = Color.HSVToRGB(h, s, v).Alpha(color.a);
        SetColor(newColor);
    }
}
