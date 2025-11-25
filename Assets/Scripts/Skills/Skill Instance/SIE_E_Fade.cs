using HyperQuest.EasyPooling;
using UnityEngine;

public class SIE_E_Fade : SIE, IPoolable
{
    const float _endTime = 0.2f;
    TickTimer _endTimer = new();
    Rigidbody2D _rb;

    protected override void Awake()
    {
        base.Awake();

        _rb = GetComponent<Rigidbody2D>();
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
        //var alpha = percent.RemapPercent()

        // Scale
        transform.localScale = transform.localScale.Lerp(transform.localScale * 0.5f, Time.deltaTime / _endTime);

        // Destroy
        if(percent >= 1f) _skillInstance.gameObject.DestroyOrRecycle();
    }
}
