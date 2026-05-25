using ManaSprite.EasyPooling;
using UnityEngine;

public class SIE_E_Fade : SIE, IPoolable
{
    float _endTime => Constants.SkillStats.BaseFadeTime;
    TickTimer _endTimer = new();
    Rigidbody2D _rb;

    ColorController _colorController;

    float _initAlpha;

    protected override void Awake()
    {
        base.Awake();

        _rb = GetComponent<Rigidbody2D>();
        _colorController = GetComponentInParent<ColorController>();

        _skillInstance.OnEnd += () =>
        {
            _initAlpha = _colorController.Color.a;
            if (_rb != null) _rb.linearVelocity *= 0f;
        };
    }

    public void Initialize()
    {
        _endTimer.Reset(_endTime);
        _skillInstance.EndPercent = 0f;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.End) return;

        // Tick Timer
        _endTimer.Tick(Time.deltaTime);
        var percent = _endTimer.GetPercent();
        _skillInstance.EndPercent = percent;

        // Color Fade
        float lerpSpeed = 25f;
        var alpha = percent.RemapPercent(_initAlpha * 0.5f, 0f);
        _colorController.SetColor(_colorController.Color.SetSaturation(_colorController.Color.GetSaturation().Lerp(0f, lerpSpeed * Time.deltaTime)).Alpha(_colorController.Color.a));
        _colorController.SetAlpha(_colorController.Color.a.Lerp(alpha, lerpSpeed * Time.deltaTime));

        // Scale
        transform.SetLossyScale(transform.lossyScale.Lerp(transform.lossyScale * 0.5f, Time.deltaTime / _endTime));

        // Destroy
        if(percent >= 1f) _skillInstance.gameObject.DestroyOrRecycle();
    }
}
