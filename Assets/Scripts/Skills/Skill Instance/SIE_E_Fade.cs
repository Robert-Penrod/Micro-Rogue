using HyperQuest.EasyPooling;
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
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.End) return;

        // Tick Timer
        _endTimer.Tick(Time.deltaTime);
        var percent = _endTimer.GetPercent();

        // Alpha Fade
        var alpha = percent.RemapPercent(_initAlpha, 0f);
        _colorController.SetAlpha(alpha);

        // Scale
        transform.SetLossyScale(transform.lossyScale.Lerp(transform.lossyScale * 0.5f, Time.deltaTime / _endTime));

        // Destroy
        if(percent >= 1f) _skillInstance.gameObject.DestroyOrRecycle();
    }
}
