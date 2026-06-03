using UnityEngine;

[RequireComponent(typeof(SimpleMenu))]
public class SimpleMenuCamZoom : MonoBehaviour
{
    [SerializeField] float _zoom = 1f;
    SimpleMenu _simpleMenu;

    private void Start()
    {
        _simpleMenu = GetComponent<SimpleMenu>();
        _simpleMenu.OnOpenChanged += (bool isOpen) =>
        {
            CameraManager.I.Zoom(isOpen ? _zoom : 1f, this);
        };
    }
}
