using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    #region Vars
    [SerializeField] string _text;
    public Sprite Sprite;
    public Color MainColor = Color.clear;
    [SerializeField] TextMeshProUGUI _textMesh;
    [SerializeField] Image _image;

    float _lerpSpeed = 25f;
    Vector3 _initScale;

    [Header("Audio")]
    public AudioClip SelectSound;
    public AudioClip SubmitSound;
    AudioSource _audioSource;

    [Header("Reference")]
    [SerializeField] List<Image> BgImages = new();
    [SerializeField] Transform _content;
    float _initContentSize;
    [SerializeField] GameObject _highlightGFX;

    [Header("State")]
    [SerializeField] bool _isInteractable = true;
    public bool IsSelected { get; private set; }
    float _submitPulse = 0f;
    public bool IsHighlighted
    {
        get
        {
            return _isHighlighted;
        }
        set
        {
            _isHighlighted = value;
            if(_isHighlighted)
            {
                OnHighlightEvent?.Invoke();
            }
            if(_highlightGFX != null) _highlightGFX.SetActive(_isHighlighted);
            if (_isHighlighted) transform.SetAsLastSibling();
        }
    }
    bool _isHighlighted;

    [Header("Events")]
    public UnityEvent OnSubmitEvent;
    public UnityEvent OnDownEvent;
    public UnityEvent OnHighlightEvent;
    #endregion

    #region Init
    private void OnValidate()
    {
        RefreshUI();
        if (_text != string.Empty) this.gameObject.name = _text + "_btn";
    }

    public void RefreshUI()
    {
        if (_textMesh != null) _textMesh.text = _text;
        if (_image != null)
        {
            _image.sprite = Sprite;
            // Color
            if (MainColor != Color.clear)
            {
                _image.color = MainColor;
                BgImages.ForEach(sr =>
                {
                    sr.color = sr.color.SetHS(MainColor.GetHue(), 0.5f * MainColor.GetSaturation());
                });
            }
        }
        _image.enabled = Sprite != null;
    }

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 0f;
        _initScale = transform.localScale;
        _initContentSize = _content.localScale.x;
    }
    #endregion

    #region UI
    public void OnSelect(BaseEventData eventData)
    {
        if (!_isInteractable) return;
        SetSelected(true);
        PlayAudio(SelectSound);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!_isInteractable) return;
        SetSelected(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInteractable) return;
        //SelectSelf();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_isInteractable) return;
        //SelectSelf();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isInteractable) return;
        if (!Input.GetMouseButtonDown(0)) return;
        SelectSelf();
        OnSubmit(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnSubmit(BaseEventData eventData)
    {
        this.DelayedInvoke(-1, () =>
        {
            if (!_isInteractable) return;
            if (!IsSelected) return;
            _submitPulse += 1f;
            //Debug.Log("Submit: " + this.gameObject.name);

            // Lock Selection
            SetInputSystemActive(false);

            // Down Event
            OnDownEvent?.Invoke();

            this.DelayedInvoke(0.2f, () =>
            {
                // Unlock Selection
                SetInputSystemActive(true);

                OnSubmitEvent?.Invoke();
            });

            PlayAudio(SubmitSound);
        });
    }

    void SetInputSystemActive(bool isActive)
    {
        EventSystem.current.sendNavigationEvents = isActive;
    }

    void SetSelected(bool isSelected)
    {
        //Debug.Log("Set Selected: " + this.gameObject.name + " " + isSelected);
        this.IsSelected = isSelected;
    }

    void PlayAudio(AudioClip clip)
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("Button has no audio source?");
            return;
        }
        _audioSource.volume = 1f;
        _audioSource.pitch = 1f + 0.1f * Random.Range(-1f, 1f);
        _audioSource.PlayOneShot(clip);
    }

    void SelectSelf()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_submitPulse > 0f) _submitPulse = _submitPulse.Lerp(0f, 0.1f * _lerpSpeed * Time.deltaTime);

        // Scale Lerp
        float targetScaleMult = IsSelected ? 1.1f : 1f;
        targetScaleMult *= _submitPulse.RemapPercent(1f, 1.1f, false);
        targetScaleMult *= IsHighlighted ? 1.1f : 1f;
        float targetX = targetScaleMult * _initScale.x;
        transform.localScale = transform.localScale.x.Lerp(targetX, _lerpSpeed * Time.deltaTime) * Vector3.one;
        if(_content != null)
        {
            float contentTargetX = targetScaleMult * _initContentSize;
            contentTargetX *= IsHighlighted ? 1.025f : 1f;
            _content.transform.localScale = _content.transform.localScale.x.Lerp(contentTargetX, _lerpSpeed * Time.deltaTime) * Vector2.one;
        }
    }
    #endregion
}
