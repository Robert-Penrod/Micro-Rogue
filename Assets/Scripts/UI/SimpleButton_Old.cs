using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimpleButton_Old : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler
{
    public bool IsInteractable = true;
    [SerializeField] AudioClip _chargeSound;
    [SerializeField] AudioClip _selectSound;
    [SerializeField] AudioClip _submitSound;

    bool _isSelected;
    float _lerpSpeed = 25f;
    float _submitTick;
    float _submitTime = 0.375f;
    float _submitPercent => (_submitTick / _submitTime).Clamp01();

    Player _currentUIOwner => PlayerManager.I.CurrentUIOwner;
    bool _pointerPresenceSelection = true;//> PlayerManager.I.CurrentUIOwner == null || _uiOwnerControlScheme == "WASD";

    AudioSource _audioSource;
    AudioSource _chargeSource;

    public UnityEvent OnSubmitEvent;

    private void Awake()
    {
        _audioSource = this.gameObject.AddComponent<AudioSource>();
        _chargeSource = this.gameObject.AddComponent<AudioSource>();
        _chargeSource.clip = _chargeSound;
        _chargeSource.spatialBlend = 0f;
        _chargeSource.loop = true;
        _chargeSource.volume = 0f;
        _chargeSource.Play();
    }

    #region UI
    public void OnSelect(BaseEventData eventData)
    {
        if (!IsInteractable) return;
        _isSelected = true;
        PlayAudio(_selectSound);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!IsInteractable) return;
        _isSelected = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        OnSubmit(eventData);
    }
    #endregion

    public void OnSubmit(BaseEventData eventData)
    {
        /*
        _submitCharge += 1f;
        PlayAudio(_submitSound);
        //OnSubmitEvent?.Invoke();
        */
    }

    void DoSubmit()
    {
        PlayAudio(_submitSound);
        OnSubmitEvent?.Invoke();
    }

    bool _wasSubmitting;
    bool _isSubmitValid;

    private void Update()
    {
        bool isSubmitingThisFrame = _isSelected && (_currentUIOwner?.Submit.WasPressedThisFrame() ?? false);
        bool isSubmiting = _isSelected && (_currentUIOwner?.Submit.IsPressed() ?? false);
        float targetPitch = 0.8f;
        float targetVolume = 1f;
        float lerpSpeed = 12f;

        if(isSubmiting)
        {
            if (isSubmitingThisFrame) _isSubmitValid = true;
            
            if(!_wasSubmitting)
            {
                _chargeSource.Stop();
                _chargeSource.Play();
            }

            if (_isSubmitValid)
            {
                targetPitch *= _submitPercent.RemapPercent(0.9f, 1.1f);
                targetVolume *= 1f;

                if (_submitTick < _submitTime)
                {
                    _submitTick += 1f * Time.deltaTime;
                    if (_submitTick >= _submitTime)
                    {
                        _wasSubmitting = false;
                        _chargeSource.Stop();
                        _submitTick = 0f;
                        DoSubmit();
                    }
                }
            }
        }
        else
        {
            _isSubmitValid = false;
            targetVolume *= 0f;
            if (_submitTick > 0f) _submitTick = (_submitTick - 4f * Time.deltaTime).Clamp01();
        }

        _chargeSource.pitch = _chargeSource.pitch.Lerp(targetPitch, lerpSpeed * Time.deltaTime);
        _chargeSource.volume = _chargeSource.volume.Lerp(targetVolume, lerpSpeed * Time.deltaTime);

        float targetScale = _isSelected ? 1.1f : 1f;


        targetScale += 0.1f * _submitPercent * (_submitPercent >= 1f? 1.1f : 1f);

        float lerpScale = transform.localScale.x.Lerp(targetScale, _lerpSpeed * Time.deltaTime);
        transform.localScale = lerpScale * Vector3.one;

        _wasSubmitting = isSubmiting;
    }

    void PlayAudio(AudioClip clip)
    {
        _audioSource.volume = 1f;
        _audioSource.pitch = 1f + 0.1f * Random.Range(-1f, 1f);
        _audioSource.PlayOneShot(clip);
    }
}
