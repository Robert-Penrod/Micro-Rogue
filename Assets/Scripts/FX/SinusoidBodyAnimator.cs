using ManaSprite.SinusoidAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SinusoidAnimator))]
public class SinusoidBodyAnimator : MonoBehaviour
{
    public float BodySpeedMin = 1f;
    public float BodySpeedMax = 4f;
    public float FreqMin = 1f;
    public float FreqMax = 4f;
    float _lerpFreq = 0f;
    float _lerpSpeed = 10f;
    SinusoidAnimator _sinusoidAnimator;
    Rigidbody2D _body;

    private void Awake()
    {
        _sinusoidAnimator = GetComponent<SinusoidAnimator>();
        _body = GetComponentInParent<Rigidbody2D>();
        _lerpFreq = FreqMin;
    }

    private void Update()
    {
        if (_body == null) return;
        float freq = _body.linearVelocity.magnitude.Remap(BodySpeedMin, BodySpeedMax, FreqMin, FreqMax, false);
        _lerpFreq = Mathf.Lerp(_lerpFreq, freq, _lerpSpeed * Time.deltaTime);
        if (_lerpFreq < FreqMin) _lerpFreq = FreqMin;
        _sinusoidAnimator.Freq = _lerpFreq;
    }
}
