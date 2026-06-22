using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SimpleButtonFX : MonoBehaviour
{
    public float LerpSpeed = 25f;
    public float SelectValueShift = 0f;
    SimpleButton _simpleButton;
    Image _image;
    Color _initColor;

    private void Awake()
    {
        _simpleButton = GetComponentInParent<SimpleButton>();
        _image = GetComponent<Image>();
        _initColor = _image.color;
    }

    private void LateUpdate()
    {
        // Color
        Color targetColor = _initColor;
        if(_simpleButton.IsSelected) targetColor = targetColor.SetValue(targetColor.GetValue() + SelectValueShift);
        _image.color = _image.color.Lerp(targetColor, LerpSpeed * Time.deltaTime);
    }
}
