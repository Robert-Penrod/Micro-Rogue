using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SimpleButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    #region Vars
    [Header("State")]
    [field: SerializeField]
    public bool CanSubmit = true;
    public bool IsSelected { get; private set; }
    public bool IsHovered { get; private set; }

    [Header("Events")]
    public Action OnSubmit;

    [Header("Audio")]
    [SerializeField] AudioClip SelectSound;
    [SerializeField] AudioClip HoverSound;
    [SerializeField] AudioClip SubmitSound;
    float _lastHoverTime;

    [Header("Reference")]
    Button _button;
    float _selectionTime;
    bool _wasSelectedThisFrame => _selectionTime == Time.unscaledTime;
    #endregion



    private void Awake()
    {
        _button = GetComponent<Button>();
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
        AudioSpawner.PlayAudioWithRandPitch(SelectSound, 0.2f, 1f, 0.75f);
        _selectionTime = Time.unscaledTime;
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
            if(Time.time - _lastHoverTime >= 0.1f) AudioSpawner.PlayAudioWithRandPitch(HoverSound, 0.2f, 1f, 0.25f);
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
    void ISubmitHandler.OnSubmit(BaseEventData eventData)
    {
        HandleSubmit();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_wasSelectedThisFrame)
        {
            HandleSubmit();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    void HandleSubmit()
    {
        if (!CanSubmit) return;
        OnSubmit?.Invoke();
        AudioSpawner.PlayAudioWithRandPitch(SubmitSound, 0.2f, 1f, 1f);
    }
    #endregion
}
