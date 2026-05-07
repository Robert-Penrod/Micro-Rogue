using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BrushTrigger : MonoBehaviour
{
    public float Vol = 0.125f;
    public AudioClip Sound;
    public float LerpSpeed = 10f;
    [Min(0f)] public float MinSpeed = 3f;
    [Min(0f)] public float HitboxDelay = 0.25f;
    float _hitboxTick = 0f;
    [Range(0f, 1f)] public float HitboxVariance = 0f;
    public float RotationAmp = 5f;
    public float PosAmp = 0.025f;
    float _targetRotationAngle = 0f;
    float _initRotationAngle = 0f;

    Vector3 _targetLocalPos = Vector3.zero;

    public UnityEvent OnTrigger;



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_hitboxTick > 0)
        {
            return;
        }

        Rigidbody2D collidingBody = collision.GetComponent<Rigidbody2D>();
        if (collidingBody == null) return;

        if (collidingBody.linearVelocity.magnitude > MinSpeed)
        {
            Trigger();
        }
    }

    private void Awake()
    {
        _initRotationAngle = transform.rotation.eulerAngles.z;
        _targetRotationAngle = _initRotationAngle;
    }

    private void Update()
    {
        if (_hitboxTick > 0)
        {
            _hitboxTick -= Time.deltaTime;
        }
        Lerp();
    }

    void Lerp()
    {
        float currentAngle = transform.rotation.eulerAngles.z;
        float lerpAngle = Mathf.LerpAngle(currentAngle, _targetRotationAngle, LerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, lerpAngle);

        Transform gfxHolder = transform.GetChild(0);
        Vector3 currentLocalPos = gfxHolder.transform.localPosition;
        Vector2 lerpPos = Vector2.Lerp(currentLocalPos, _targetLocalPos, LerpSpeed * Time.deltaTime);
        gfxHolder.localPosition = (Vector3)lerpPos + Vector3.forward * gfxHolder.localPosition.z;
    }

    void Trigger()
    {
        _hitboxTick = HitboxDelay + HitboxVariance * Random.Range(-1f, 1f) * HitboxDelay;
        
        AudioSpawner.PlayAudioWithRandPitch(Sound, 0.2f, 1f, 0.25f * Vol, (Vector2)transform.position).spatialBlend = 0.6f;

        _targetRotationAngle = _initRotationAngle + Random.Range(-1f, 1f) * RotationAmp;
        _targetLocalPos = Random.insideUnitCircle * PosAmp * transform.localScale.x;

        OnTrigger?.Invoke();
    }
}
