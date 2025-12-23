using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSingleton : PersistantSingleton<EventSystemSingleton>
{
    GameObject _currentSelected;
    GameObject _lastSelected;

    private void Update()
    {
        _currentSelected = EventSystem.current.currentSelectedGameObject;

        if(_currentSelected == null && _lastSelected != null)
        {
            _currentSelected = _lastSelected;
            EventSystem.current.SetSelectedGameObject(_lastSelected);
        }

        if (_currentSelected != null)
        {
            _lastSelected = _currentSelected;
        }
    }
}
