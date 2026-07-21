using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectEventer : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Action<BaseEventData> OnSelectEvent;
    public Action<BaseEventData> OnDeselectEvent;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectEvent?.Invoke(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!_button.IsInteractable()) return;
        OnSelectEvent?.Invoke(eventData);
    }
}
