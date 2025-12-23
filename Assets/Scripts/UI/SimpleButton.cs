using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler
{
    [SerializeField] AudioClip _selectSound;
    [SerializeField] AudioClip _submitSound;

    bool _isSelected;
    float _lerpSpeed = 25f;
    float _submitCharge;

    bool _pointerPresenceSelection = true;

    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = this.gameObject.GetOrAddComponent<AudioSource>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
        PlayAudio(_selectSound);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData) => OnSubmit(eventData);
    public void OnSubmit(BaseEventData eventData)
    {
        _submitCharge += 1f;
        PlayAudio(_submitSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_pointerPresenceSelection) EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    private void Update()
    {
        if (_submitCharge > 0f) _submitCharge = (_submitCharge - (Time.deltaTime / 0.25f)).Clamp01();
        float targetScale = _isSelected ? 1.1f : 1f;
        targetScale += 0.1f * _submitCharge;
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
