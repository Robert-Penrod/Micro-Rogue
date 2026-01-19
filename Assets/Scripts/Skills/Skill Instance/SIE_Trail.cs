using ManaSprite.EasyPooling;
using UnityEngine;

public class SIE_Trail : SIE, IPoolable
{
    float _startScale;

    public void Initialize()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        _skillInstance.OnActivated += Place;
    }

    void Place()
    {
        transform.SetParent(null);
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Activated)
        {
            _startScale = transform.localScale.x;
            return;
        }

        float scale = _skillInstance.ActivePercent.RemapPercent(_startScale, 0.75f * _startScale);
        transform.localScale = scale * Vector3.one;
    }
}
