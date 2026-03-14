using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SIE_TelegraphSprite : SIE
{
    SpriteRenderer _spriteRend;
    float _lerpSpeed = 32f;

    protected override void Awake()
    {
        base.Awake();
        _spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SetAlpha(1f);
    }

    private void Update()
    {
        float targetAlpha = _skillInstance.State == SkillInstance.SkillInstanceState.Start ? 1f : 0f;
        float lerpAlpha = _spriteRend.color.a.Lerp(targetAlpha, _lerpSpeed * Time.deltaTime);
        SetAlpha(lerpAlpha);
    }

    void SetAlpha(float alpha)
    {
        _spriteRend.color = _spriteRend.color.Alpha(alpha);
    }
}
