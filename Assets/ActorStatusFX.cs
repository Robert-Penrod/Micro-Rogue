using UnityEngine;

public class ActorStatusFX : MonoBehaviour
{
    [SerializeField] float _maxAlpha = 1f;
    public enum StatusTypeEnum { Pyro, Frost, Static };
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
        float effectMag = StatusType switch
        {
            StatusTypeEnum.Pyro => _actor.PyroPercent.Pow(3f),
            StatusTypeEnum.Frost => _actor.FrostPercent,
            StatusTypeEnum.Static => _actor.StaticPercent.Pow(6f),
            _ => 0f
        };

        _spriteRend.color = _spriteRend.color.Alpha(_spriteRend.color.a.Lerp(effectMag * _maxAlpha, lerpSpeed * Time.deltaTime));
        _spriteRend.transform.localScale = Vector3.one * _spriteRend.transform.localScale.x.Lerp(effectMag.RemapPercent(0.375f, 1f), lerpSpeed * Time.deltaTime);
    }
}
