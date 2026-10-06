using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UpgradeGridUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SimpleMenu _startRunMenu;

    CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetStartMenuOpen(bool isOpen)
    {
        _startRunMenu.SetOpen(isOpen);
        _canvasGroup.interactable = isOpen;
    }
}
