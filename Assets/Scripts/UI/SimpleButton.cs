using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler
{
    [SerializeField] AudioClip _selectSound;
    [SerializeField] AudioClip _submitSound;

    bool _isSelected;
    float _lerpSpeed = 25f;
    float _submitTick;
    float _submitTime = 0.5f;

    Player _currentUIOwner => PlayerManager.I.CurrentUIOwner;
    bool _pointerPresenceSelection = true;//> PlayerManager.I.CurrentUIOwner == null || _uiOwnerControlScheme == "WASD";

    AudioSource _audioSource;

    public UnityEvent OnSubmitEvent;

    private void Awake()
    {
        _audioSource = this.gameObject.GetOrAddComponent<AudioSource>();
    }

    #region UI
    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
        PlayAudio(_selectSound);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    public void OnPointerDown(PointerEventData eventData) => OnSubmit(eventData);
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
    
    private void Update()
    {
        bool isSubmiting = _isSelected && (_currentUIOwner?.Submit.IsPressed() ?? false);

        if(isSubmiting)
        {
            if (_submitTick < _submitTime)
            {
                _submitTick += Time.deltaTime;
                if(_submitTick >= _submitTime)
                {
                    DoSubmit();
                }
            }
        }
        else
        {
            if (_submitTick > 0f) _submitTick = (_submitTick - 2f * Time.deltaTime).Clamp01();
        }

        float targetScale = _isSelected ? 1.1f : 1f;


        targetScale += 0.1f * (_submitTick / _submitTime).Clamp01();

        float lerpScale = transform.localScale.x.Lerp(targetScale, _lerpSpeed * Time.deltaTime);
        transform.localScale = lerpScale * Vector3.one;
    }

    void PlayAudio(AudioClip clip)
    {
        _audioSource.volume = 1f;
        _audioSource.pitch = 1f + 0.1f * Random.Range(-1f, 1f);
        _audioSource.PlayOneShot(clip);
    }
}
