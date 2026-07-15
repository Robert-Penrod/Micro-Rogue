using UnityEngine;

public class ActorStatusFX : MonoBehaviour
{
    public enum StatusTypeEnum { Frost };
    public StatusTypeEnum StatusType;

    Actor _actor;
    SpriteRenderer _spriteRend;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _spriteRend = GetComponentInParent<SpriteRenderer>();
    }

    private void Update()
    {
        float lerpSpeed = 12f;
        float effectMag = _actor.FrostPercent;
        _spriteRend.color = _spriteRend.color.Alpha(_spriteRend.color.a.Lerp(effectMag, lerpSpeed * Time.deltaTime));
        _spriteRend.transform.localScale = Vector3.one * _spriteRend.transform.localScale.x.Lerp(effectMag.RemapPercent(0.375f, 1f), lerpSpeed * Time.deltaTime);
    }
}
