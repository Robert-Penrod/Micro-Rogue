using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton_OldUI : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
{
    #region Vars
    [SerializeField] string _text;
    public Sprite Sprite;
    public Color MainColor = Color.clear;
    [SerializeField] TextMeshProUGUI _textMesh;
    [SerializeField] Image _image;
    public Transform LevelPipHolder;

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

    
    public bool GetInteractable()
    {
        return _button.interactable;
    }
    public void SetInteractable(bool interactableState)
    {
        if (_button != null) _button.interactable = interactableState;
        RefreshUI();
    }
    [Header("State")]
    Button _button;
    public bool IsSelected { get; private set; }
    float _submitPulse = 0f;
    public bool IsHovered { get; private set; }
    public bool IsHighlighted
    {
        get
        {
            return _isHighlighted;
        }
        set
        {
            if (Time.time < 1f) return;
            _isHighlighted = value;
            if(_isHighlighted)
            {
                OnHighlightEvent?.Invoke();
            }
            if(_highlightGFX != null) _highlightGFX.SetActive(_isHighlighted);
            //if (_isHighlighted) transform.SetAsLastSibling();
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
        if (_button == null) _button = GetComponent<Button>();
        RefreshUI();
        if (_text != string.Empty) this.gameObject.name = _text + "_btn";
    }

    public void RefreshUI()
    {
        var isInteractable = GetInteractable();
        var alpha = isInteractable ? 1f : 0.5f;

        //Debug.Log(gameObject);
        if (gameObject == null) return;
        if (_textMesh != null) _textMesh.text = _text;
        if (_image != null)
        {
            _image.sprite = Sprite;
            
            // Color
            Color c = MainColor == Color.clear ? Color.grey : MainColor;
            _image.color = c.Alpha(alpha);
            BgImages.ForEach(sr =>
            {
                sr.color = sr.color.SetHS(c.GetHue(), 0.5f * c.GetSaturation()).Alpha(alpha);
            });
            _image.enabled = Sprite != null;
        }
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 0f;
        _initScale = transform.localScale;
        _initContentSize = _content.localScale.x;
    }

    private void OnEnable()
    {
        RefreshUI();
    }
    #endregion

    #region UI
    public void OnSelect(BaseEventData eventData)
    {
        if (!GetInteractable()) return;
        SetSelected(true);
        PlayAudio(SelectSound);
        RefreshUI();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!GetInteractable()) return;
        SetSelected(false);
        RefreshUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetInteractable()) return;
        IsHovered = true;
        //SelectSelf();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!GetInteractable()) return;
        //SelectSelf();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GetInteractable()) return;
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
        //Debug.Log("BUTTON SUBMIT START");
        this.DelayedInvoke(-1, () =>
        {
            //Debug.Log("BUTTON DELAY 1");
            if (!GetInteractable()) return;
            if (!IsSelected) return;
            _submitPulse += 1f;
            //Debug.Log("Submit: " + this.gameObject.name);

            // Lock Selection
            SetInputSystemActive(false);

            // Down Event
            OnDownEvent?.Invoke();

            this.DelayedInvoke(0.2f, () =>
            {
                //Debug.Log("BUTTON SUBMIT");
                // Unlock Selection
                SetInputSystemActive(true);

                OnSubmitEvent?.Invoke();
            });

            PlayAudio(SubmitSound);
        });
        //Debug.Log("BUTTON SUBMIT END");
    }

    void SetInputSystemActive(bool isActive)
    {
        EventSystem.current.sendNavigationEvents = isActive;
    }

    bool GetInputSystemActive() => EventSystem.current.sendNavigationEvents;

    void SetSelected(bool isSelected)
    {
        if (!GetInteractable()) return;
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
        targetScaleMult *= IsHovered ? 1.1f : 1f;
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
