using TMPro;
using UnityEngine;

public class FontHandler : MonoBehaviour
{
    TextMeshProUGUI _textMeshUI;

    private void Awake()
    {
        _textMeshUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateFont();
    }

    private void OnValidate()
    {
        _textMeshUI = GetComponent<TextMeshProUGUI>();
        UpdateFont();
    }

    public void UpdateFont()
    {
        if (FontManager.I == null) return;
        SetFont(FontManager.I.Font);
    }

    public void SetFont(TMP_FontAsset font)
    {
        if (_textMeshUI == null) _textMeshUI = GetComponent<TextMeshProUGUI>();
        _textMeshUI.font = font;
    }
}
