using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CC_Child : MonoBehaviour
{
    [SerializeField] bool _useColor = true;
    [SerializeField] bool _useAlpha = true;
    [SerializeField] bool _useOwnColorHueSat = false;
    public float SaturationOffset = 0f;
    public float ValueOffset = 0f;

    ColorController _colorController;
    SpriteRenderer _spriteRend;
    ParticleSystem _pSystem;
    Image _image;
    float _initAlpha;


    private void Awake()
    {
        // References
        _colorController = GetComponentInParent<ColorController>();
        _spriteRend = GetComponent<SpriteRenderer>();
        _pSystem = GetComponent<ParticleSystem>();
        _image = GetComponent<Image>();

        // Cache starting values
        _initAlpha = GetColor().a;

        // Events
        _colorController.OnColorChanged += (Color c) =>
        {
            UpdateColor(c);
        };

        _colorController.OnAlphaChanged += (float a) =>
        {
            UpdateAlpha(a);
        };

        UpdateColorAndAlpha(_colorController.Color);
    }

    void UpdateColorAndAlpha(Color newColor)
    {
        UpdateColor(newColor);
        UpdateAlpha(newColor.a);
    }

    void UpdateColor(Color newColor)
    {
        if (!_useColor) return;
        Color.RGBToHSV(newColor, out float h, out float s, out float v);
        Color offsetColor = Color.HSVToRGB(h, s + SaturationOffset, v + ValueOffset);
        if(_useOwnColorHueSat)
        {
            Color.RGBToHSV(GetColor(), out float selfH, out float selfS, out float selfV);
            offsetColor = Color.HSVToRGB(selfH, selfS, v + ValueOffset);
        }
        SetColor(offsetColor.Alpha(GetColor().a));
    }

    void UpdateAlpha(float alpha)
    {
        if (!_useAlpha) return;
        float calculatedAlpha = alpha.Remap(0f, 1f, 0f, _initAlpha);
        SetColor(GetColor().Alpha(calculatedAlpha));
    }

    void SetColor(Color c)
    {
        if (_spriteRend != null) _spriteRend.color = c;
        if (_pSystem != null)
        {
            ParticleSystem.MainModule main = _pSystem.main;
            main.startColor = c;
        }
        if (_image != null)
        {
            _image.color = c;
        }
    }

    Color GetColor()
    {
        if (_spriteRend != null) return _spriteRend.color;
        if (_pSystem != null)
        {
            ParticleSystem.MainModule main = _pSystem.main;
            return main.startColor.color;
        }
        if (_image != null) return _image.color;
        return Color.black;
    }
}
