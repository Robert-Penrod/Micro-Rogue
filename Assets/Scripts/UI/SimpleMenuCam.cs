using UnityEngine;

[RequireComponent(typeof(SimpleMenu))]
public class SimpleMenuCam : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _zoom = 1f;
    [SerializeField] Vector2 _offset;
    SimpleMenu _simpleMenu;

    private void Start()
    {
        _simpleMenu = GetComponent<SimpleMenu>();
        _simpleMenu.OnOpenChanged += (bool isOpen) =>
        {
            UpdateCam(isOpen); 
        };
        UpdateCam(_simpleMenu.IsOpen);
    }

    void UpdateCam(bool isOpen)
    {
        CameraManager.I.Zoom(isOpen ? _zoom : 1f, this);
        CameraManager.I.Offset(isOpen ? _offset : Vector2.zero, this);
    }
}
