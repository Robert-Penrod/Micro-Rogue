using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSingleton : PersistantSingleton<EventSystemSingleton>
{
    GameObject _currentSelected;
    public GameObject _lastSelected { get; private set; }

    private void Update()
    {
        _currentSelected = EventSystem.current.currentSelectedGameObject;

        
        if(_currentSelected == null && _lastSelected != null)
        {
            this.DelayedInvoke(-1f, () =>
            {
                Debug.Log("_currentSelected == null, selecting last selection " + _lastSelected.gameObject.name);
                _currentSelected = _lastSelected;
                EventSystem.current.SetSelectedGameObject(_lastSelected);
            });
        }
        

        if (_currentSelected != null)
        {
            _lastSelected = _currentSelected;
        }
    }

    public void ClearSelection()
    {
        _lastSelected = _currentSelected = null;
        EventSystem.current.SetSelectedGameObject(null);
    }
}
