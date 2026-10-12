using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SimpleMenu))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] List<SimpleMenu> _menusToOpen = new();
    SimpleMenu _thisMenu;

    void SkipMenu()
    {
        PlayerPrefs.SetInt("Restarting", 0);
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

        bool doSkip = PlayerPrefs.GetInt("Restarting", 0) > 0;
        if (doSkip) SkipMenu();
    }
}
