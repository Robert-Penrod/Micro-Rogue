using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleMenu : MonoBehaviour
{
    [SerializeField] bool StartOpen;
    [SerializeField] SimpleMenu _prevMenu;
    float _lerpSpeed = 12f;
    public bool IsOpen { get; private set; }

    CanvasGroup _canvasGroup;

    public UnityEvent OnGoBack;
    public Action<bool> OnOpenChanged;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (StartOpen) SetOpen(true);
        else SetOpen(false);
        _canvasGroup.alpha = IsOpen ? 1f : 0f;
    }

    private void Update()
    {
        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(IsOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);

        if(IsOpen)
        {
            // Back
            if(_prevMenu != null && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
            {
                SwitchMenu(_prevMenu);
                OnGoBack?.Invoke();
            }
        }
    }

    public void SetOpen(bool isOpen)
    {
        bool didOpenChange = this.IsOpen != isOpen;
        this.IsOpen = isOpen;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = isOpen;

        if(didOpenChange) OnOpenChanged?.Invoke(isOpen);
    }

    public void SwitchMenu(SimpleMenu otherMenu)
    {
        this.SetOpen(false);
        otherMenu.SetOpen(true);
    }
}
