using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SimpleMenu : MonoBehaviour
{
    [SerializeField] bool StartOpen;
    [SerializeField] Button _selectOnOpen;
    [SerializeField] SimpleMenu _prevMenu;
    float _lerpSpeed = 12f;
    public bool IsOpen { get; private set; }

    CanvasGroup _canvasGroup;

    public UnityEvent OnGoBack;
    public Action<bool> OnOpenChanged;


    private void Awake()
    {
        _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (StartOpen) SetOpen(true);
        else SetOpen(false);
        _canvasGroup.alpha = IsOpen ? 1f : 0f;
    }

    protected virtual void Update()
    {
        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(IsOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);
        //if (!IsOpen && _canvasGroup.alpha < 0.001f) this.gameObject.SetActive(false); 

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
        if (isOpen) this.gameObject.SetActive(true);

        bool didOpenChange = true;// this.IsOpen != isOpen;
        this.IsOpen = isOpen;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = isOpen;

        if(didOpenChange)
        {
            if(isOpen)
            {
                if(_selectOnOpen != null) EventSystem.current.SetSelectedGameObject(_selectOnOpen.gameObject);
            }

            OnOpenChanged?.Invoke(isOpen);
        }
    }

    public void SwitchMenu(SimpleMenu otherMenu)
    {
        this.SetOpen(false);
        otherMenu.SetOpen(true);
    }

    public void GoBack()
    {
        SwitchMenu(_prevMenu);
        OnGoBack?.Invoke();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
