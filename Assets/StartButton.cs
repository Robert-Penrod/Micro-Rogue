using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(SimpleButton))]
public class StartButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image _bg;
    [SerializeField] SimpleMenu _startModal;
    Color _initBgColor;

    SimpleButton _simpleButton;
    float _submitTick;

    public UnityEvent OnSubmitEvent;

    private void Awake()
    {
        _simpleButton = GetComponent<SimpleButton>();
        _simpleButton.OnSubmit += () =>
        {
            if (_startModal.IsOpen) return;

            _submitTick += 0.5f;
            this.DelayedInvoke(0.375f, () =>
            {
                HandleSubmit();
            });
        };

        _initBgColor = _bg.color;
    }

    void HandleSubmit()
    {
        //DungeonManager.I.StartGame();
        //_startModal.SetOpen(true);
        OnSubmitEvent?.Invoke();
    }

    private void Update()
    {
        // Tick
        if (_submitTick > 0f) _submitTick = (_submitTick - Time.deltaTime).ClampMin(0f);

        // Params
        float lerpSpeed = 12f;

        // Size
        float targetSize = 1f;
        targetSize *= _submitTick.Remap(0f, 0.5f, 1f, 1.2f, false);
        if(_submitTick <= 0.01f) targetSize *= _simpleButton.IsHovered ? 1.1f : 1f;
        transform.localScale = transform.localScale.Lerp(Vector3.one * targetSize, lerpSpeed * Time.deltaTime);

        // Color
        Color targetColor = _initBgColor;
        float lerp = 0f;
        if (_simpleButton.IsHovered) lerp += 0.5f;
        lerp += _submitTick.Remap(0f, 0.5f, 0f, 1f);
        lerp = lerp.Clamp01();
        targetColor = targetColor.Lerp(Color.white, lerp);
        _bg.color = _bg.color.Lerp(targetColor, lerpSpeed * Time.deltaTime);

        // Input
        if (_simpleButton._button.IsInteractable())
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _simpleButton.TriggerSubmit();
            }

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad.buttonWest.wasPressedThisFrame)
                {
                    _simpleButton.TriggerSubmit();
                }
            }
        }
    }
}
