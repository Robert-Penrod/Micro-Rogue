using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    Image _image;
    Button _button;
    bool _isActive;
    float _lerpSpeed = 25f;

    private void Awake()
    {
        // Init
        _image = GetComponentInChildren<Image>();
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
        _isActive = isActive;

        if(isActive)
        {
            var lastSelected = EventSystemSingleton.I?._lastSelected;
            if (lastSelected != null)
            {
                transform.position = lastSelected.transform.position.Lerp(transform.position, 0f);
            }
        }
    }

    private void LateUpdate()
    {
        //_image.color = _image.color.Alpha(_image.color.a.Lerp(_isActive? 1f : 0f, _lerpSpeed * Time.deltaTime));
        //transform.localScale = transform.localScale.x.Lerp(_isActive? 1f : 0.5f, _lerpSpeed * Time.deltaTime) * Vector3.one;
        transform.localPosition = transform.localPosition.Lerp(Vector3.zero, _lerpSpeed * Time.deltaTime);
    }
}
