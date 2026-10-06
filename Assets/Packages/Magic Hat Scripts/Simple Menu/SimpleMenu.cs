using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SimpleMenu : MonoBehaviour
{
    [SerializeField] bool StartOpen;
    [SerializeField] bool _doPreviousSelectionCache;
    [SerializeField] Button _selectOnOpen;
    [SerializeField] SimpleMenu _prevMenu;
    float _lerpSpeed = 12f;
    public bool IsOpen { get; private set; }
    bool _wentBackThisFrame = false;

    CanvasGroup _canvasGroup;

    public UnityEvent OnGoBack;
    public Action<bool> OnOpenChanged;

    public void JoinPlayerByLastInput()
    {
        PlayerManager.I.JoinPlayerByLastInputDevice();
    }

    public void UnjoinAllPlayers()
    {
        PlayerManager.I.UnjoinAllPlayers();
    }

    private void Awake()
    {
        _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (StartOpen) SetOpen(true);
        else SetOpen(false);
        _canvasGroup.alpha = IsOpen ? 1f : 0f;

        this.DelayedInvoke(-1, () =>
        {
            if (_selectOnOpen != null && StartOpen && this.gameObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(_selectOnOpen.gameObject);
            }
        });
    }

    protected virtual void Update()
    {
        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(IsOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);
        //if (!IsOpen && _canvasGroup.alpha < 0.001f) this.gameObject.SetActive(false); 

        if(IsOpen)
        {
            // Selection Cache
            if (_canvasGroup.interactable && _prevSelectionCache != EventSystem.current.currentSelectedGameObject)
            {
                _prevSelectionCache = EventSystem.current.currentSelectedGameObject;
            }

            // Back
            bool backInput = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
            if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) backInput = true; 
            if (EventSystem.current.sendNavigationEvents && !_wentBackThisFrame && _prevMenu != null && backInput)
            {
                _prevMenu._wentBackThisFrame = true;
                SwitchMenu(_prevMenu);
                OnGoBack?.Invoke();
            }
            else
            {
                _wentBackThisFrame = false;
            }
        }
    }


    GameObject _prevSelectionCache;
    public void SetOpen(bool isOpen)
    {

        if (isOpen)
        {
            this.gameObject.SetActive(true);
        }

        bool didOpenChange = true;// this.IsOpen != isOpen;
        this.IsOpen = isOpen;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = isOpen;

        if(didOpenChange)
        {
            if(isOpen && this.gameObject.activeInHierarchy)
            {
                if (_doPreviousSelectionCache && _prevSelectionCache != null) EventSystem.current.SetSelectedGameObject(_prevSelectionCache);
                else if(_selectOnOpen != null) EventSystem.current.SetSelectedGameObject(_selectOnOpen.gameObject);
            }

            OnOpenChanged?.Invoke(isOpen);
        }
    }

    public void SwitchMenu(SimpleMenu otherMenu)
    {
        this.SetOpen(false);
        foreach (var menu in FindObjectsByType<SimpleMenu>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) 
        {
            menu.SetOpen(false);
        }
        otherMenu.SetOpen(true);
    }

    public void GoBack()
    {
        if (_prevMenu == null) return;
        SwitchMenu(_prevMenu);
        OnGoBack?.Invoke();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
