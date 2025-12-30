using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class GameOverMenu : MonoBehaviour
{
    [SerializeField] GameObject _startButton;
    CanvasGroup _canvasGroup;

    bool _isOpen;
    bool _isGameOver;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SetOpen(false);
    }

    private void Update()
    {
        if(!_isGameOver && PlayerManager.I.AreAllPlayersDead())
        {
            _isGameOver = true;
            this.DelayedInvoke(1f, () =>
            {
                SetOpen(true);
                EventSystem.current.SetSelectedGameObject(_startButton);
            });
        }

        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(_isOpen ? 1f : 0f, 3f * Time.deltaTime);
    }

    void SetOpen(bool openState)
    {
        this._isOpen = openState;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = openState;
    }

    public void Confirm()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
