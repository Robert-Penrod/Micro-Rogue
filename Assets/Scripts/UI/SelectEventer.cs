using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectEventer : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Action<BaseEventData> OnSelectEvent;
    public Action<BaseEventData> OnDeselectEvent;

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectEvent?.Invoke(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectEvent?.Invoke(eventData);
    }
}
