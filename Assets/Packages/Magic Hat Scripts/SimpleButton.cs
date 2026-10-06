using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SimpleButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    #region Vars
    [Header("State")]
    [field: SerializeField]
    public bool CanSubmit = true;
    [SerializeField] float _pressTime;
    [field: SerializeField]
    public float PressPercent => _pressTick.Remap(0f, _pressTime, 0f, 1f, false);
    float _pressTick;
    public bool IsSelected { get; private set; }
    public bool IsHovered { get; private set; }

    [Header("Events")]
    public Action OnSubmit;
    public UnityEvent OnSubmitEvent;

    [Header("Audio")]
    [SerializeField] AudioClip SelectSound;
    [SerializeField] AudioClip HoverSound;
    [SerializeField] AudioClip SubmitSound;
    float _lastHoverTime;

    [Header("Reference")]
    Button _button;
    float _selectionTime;
    public bool IsSubmitedOnPointerSelect = true;
    bool _wasSelectedThisFrame => _selectionTime == Time.unscaledTime;
    #endregion



    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        if (_pressTick > 0f) _pressTick = (_pressTick - (_pressTick * Time.deltaTime)).ClampMin(0f);
    }

    public void SetActive(bool state)
    {
        if(_button != null) _button.enabled = state;
        this.enabled = state;
    }

    #region Select
    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
        PlayAudio(SelectSound, 0.75f);
        _selectionTime = Time.unscaledTime;
        transform.SetSiblingIndex(transform.parent.childCount-1);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        IsSelected = false;
    }
    #endregion

    #region Hover
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if(!IsHovered)
        {
            if(Time.time - _lastHoverTime >= 0.1f) PlayAudio(HoverSound, 0.25f);
            _lastHoverTime = Time.time;
        }
        IsHovered = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
    }
    #endregion

    #region Submit
    public void TriggerSubmit()
    {
        HandleSubmit();
    }
    void ISubmitHandler.OnSubmit(BaseEventData eventData)
    {
        HandleSubmit();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(IsSubmitedOnPointerSelect || !_wasSelectedThisFrame)
        {
            HandleSubmit();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    void HandleSubmit()
    {
        Debug.Log("Button Submitting!!");

        if (!CanSubmit) return;
        _pressTick += _pressTime;
        OnSubmit?.Invoke();
        OnSubmitEvent?.Invoke();
        PlayAudio(SubmitSound);
    }
    #endregion

    void PlayAudio(AudioClip clip, float vol = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.2f, 1f, vol);
    }
}
