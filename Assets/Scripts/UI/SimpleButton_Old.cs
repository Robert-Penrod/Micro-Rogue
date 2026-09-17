using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton_Old : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool IsInteractable = true;

    [SerializeField] AudioClip _chargeSound;
    [SerializeField] AudioClip _selectSound;
    [SerializeField] AudioClip _submitSound;

    public UnityEvent OnSubmitEvent;

    const float LERP_SPEED = 25f;
    const float SUBMIT_TIME = 0.375f;
    const float SUBMIT_CHARGE_LERP_SPEED = 12f;
    const float SUBMIT_DECAY_SPEED = 4f;

    bool _isSelected;
    bool _pointerHeld;
    float _submitTick;
    float SubmitPercent => (_submitTick / SUBMIT_TIME).Clamp01();

    Player _currentUIOwner => PlayerManager.I.CurrentUIOwner;
    bool _pointerPresenceSelection = true; // PlayerManager.I.CurrentUIOwner == null || _uiOwnerControlScheme == "WASD";

    AudioSource _audioSource;
    AudioSource _chargeSource;
    CanvasGroup _canvasGroup;

    bool _wasSubmitting;
    bool _isSubmitValid;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();

        _chargeSource = gameObject.AddComponent<AudioSource>();
        _chargeSource.clip = _chargeSound;
        _chargeSource.spatialBlend = 0f;
        _chargeSource.loop = true;
        _chargeSource.volume = 0f;
        _chargeSource.Play();

        _canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    private void OnDisable()
    {
        _isSelected = false;
        _pointerHeld = false;
        _wasSubmitting = false;
        _isSubmitValid = false;
        _submitTick = 0f;
    }

    #region UI events
    public void OnSelect(BaseEventData eventData)
    {
        if (!IsInteractable) return;
        _isSelected = true;
        PlayAudio(_selectSound);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        _pointerHeld = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        _pointerHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerHeld = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerHeld = false;
    }
    #endregion

    void DoSubmit()
    {
        PlayAudio(_submitSound);
        OnSubmitEvent?.Invoke();
    }

    private void Update()
    {
        if (_canvasGroup != null && !_canvasGroup.interactable) return;

        // Polled every frame from live input state (pointer flag or input-action state),
        // rather than a one-shot event, so "released" is always detectable.
        bool keyboardHeld = _isSelected && (_currentUIOwner?.Submit.IsPressed() ?? false);
        bool keyboardPressedThisFrame = _isSelected && (_currentUIOwner?.Submit.WasPressedThisFrame() ?? false);

        bool isSubmittingThisFrame = (_pointerHeld && !_wasSubmitting) || keyboardPressedThisFrame;
        bool isSubmitting = _pointerHeld || keyboardHeld;

        float targetPitch = 0.8f;
        float targetVolume = 1f;

        if (isSubmitting)
        {
            if (isSubmittingThisFrame) _isSubmitValid = true;

            if (!_wasSubmitting)
            {
                _chargeSource.Stop();
                _chargeSource.Play();
            }

            if (_isSubmitValid)
            {
                targetPitch *= SubmitPercent.RemapPercent(0.9f, 1.1f);

                _submitTick += Time.deltaTime;
                if (_submitTick >= SUBMIT_TIME)
                {
                    _submitTick = 0f;
                    _isSubmitValid = false;
                    isSubmitting = false; // force a clean reset this frame so it doesn't immediately recharge while still held
                    _chargeSource.Stop();
                    DoSubmit();
                }
            }
        }
        else
        {
            _isSubmitValid = false;
            targetVolume = 0f;
            if (_submitTick > 0f) _submitTick = (_submitTick - SUBMIT_DECAY_SPEED * Time.deltaTime).Clamp01();
        }

        _chargeSource.pitch = _chargeSource.pitch.Lerp(targetPitch, SUBMIT_CHARGE_LERP_SPEED * Time.deltaTime);
        _chargeSource.volume = _chargeSource.volume.Lerp(targetVolume, SUBMIT_CHARGE_LERP_SPEED * Time.deltaTime);

        float targetScale = _isSelected ? 1.1f : 1f;
        targetScale += 0.1f * SubmitPercent * (SubmitPercent >= 1f ? 1.1f : 1f);

        float lerpScale = transform.localScale.x.Lerp(targetScale, LERP_SPEED * Time.deltaTime);
        transform.localScale = lerpScale * Vector3.one;

        _wasSubmitting = isSubmitting;
    }

    void PlayAudio(AudioClip clip)
    {
        _audioSource.volume = 1f;
        _audioSource.pitch = 1f + 0.1f * Random.Range(-1f, 1f);
        _audioSource.PlayOneShot(clip);
    }
}