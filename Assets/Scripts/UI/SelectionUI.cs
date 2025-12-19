using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SelectionUI : MonoBehaviour
{
    Image _image;
    Button _button;

    private void Awake()
    {
        // Init
        _image = GetComponent<Image>();
        _button = GetComponentInParent<Button>();
        SetActive(false);

        // Events
        var selectEventer = _button.gameObject.GetOrAddComponent<SelectEventer>();
        selectEventer.OnSelectEvent += (eventData) =>
        {
            SetActive(true);
        };
        selectEventer.OnDeselectEvent += (eventData) =>
        {
            SetActive(false);
        };
    }

    void SetActive(bool isActive)
    {
        _image.enabled = isActive;
    }
}
