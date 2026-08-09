using UnityEngine;

public class ActorStatusFX : MonoBehaviour
{
    [SerializeField] AudioClip _statusSound;
    [SerializeField] float _maxAlpha = 1f;
    public enum StatusTypeEnum { Pyro, Frost, Static };
    public StatusTypeEnum StatusType;

    Actor _actor;
    SpriteRenderer _spriteRend;
    AudioSource _audioSource;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _spriteRend = GetComponentInParent<SpriteRenderer>();
        if (_statusSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 0.5f;
            _audioSource.clip = _statusSound;
            _audioSource.volume = 0f;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }

    private void Update()
    {
        float lerpSpeed = 12f;
        float effectMag = StatusType switch
        {
            StatusTypeEnum.Pyro => _actor.PyroPercent.Remap(0f, 0.75f, 0f, 1f, false).Pow(2f),
            StatusTypeEnum.Frost => _actor.FrostPercent,
            StatusTypeEnum.Static => _actor.StaticPercent.Pow(3f),
            _ => 0f
        };

        if(_audioSource != null)
        {
            _audioSource.volume = _audioSource.volume.Lerp(effectMag.Remap(0.5f, 1f, 0f, 1f), 12f * Time.deltaTime);
        }

        _spriteRend.color = _spriteRend.color.Alpha(_spriteRend.color.a.Lerp(effectMag * _maxAlpha, lerpSpeed * Time.deltaTime));
        _spriteRend.transform.localScale = Vector3.one * _spriteRend.transform.localScale.x.Lerp(effectMag.RemapPercent(0.375f, 1f), lerpSpeed * Time.deltaTime);
    }
}
