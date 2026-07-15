using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SimpleButtonFX : MonoBehaviour
{
    public float LerpSpeed = 25f;
    public float SelectValueShift = 0f;
    float _highlightValueShift = 0.125f;
    SimpleButton _simpleButton;
    Image _image;
    Color _initColor;

    private void Awake()
    {
        _simpleButton = GetComponentInParent<SimpleButton>();
        _image = GetComponent<Image>();
    }
    private void Start()
    {
        _initColor = _image.color;
    }

    private void LateUpdate()
    {
        // Color
        /*
        Color targetColor = _initColor;
        float targetValue = targetColor.GetValue();
        if (_simpleButton.IsHighlighted) targetValue += _highlightValueShift;
        if (_simpleButton.IsSelected) targetValue += SelectValueShift;
        targetColor = targetColor.SetValue(targetValue);
        _image.color = _image.color.Lerp(targetColor, LerpSpeed * Time.deltaTime).Alpha(_image.color.a);
        */
    }
}
