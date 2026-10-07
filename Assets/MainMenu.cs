using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SimpleMenu))]
public class MainMenu : MonoBehaviour
{
    static bool _notFirstOpen = false;

    [SerializeField] List<SimpleMenu> _menusToOpen = new();
    SimpleMenu _thisMenu;

    void SkipMenu()
    {
        this.DelayedInvoke(-1, () =>
        {
            _thisMenu.SetOpen(false);
            for(int i = 0; i < _menusToOpen.Count; i++)
            {
                _menusToOpen[i].SetOpen(true);
            }

            Debug.Log(EventSystem.current.currentSelectedGameObject);
        });
    }

    private void Start()
    {
        _thisMenu = GetComponent<SimpleMenu>();
        if(_notFirstOpen)
        {
            Debug.Log("Skippin");
            SkipMenu();
        }
        else
        {
            Debug.Log("First");
            _notFirstOpen = true;
        }
    }
}
