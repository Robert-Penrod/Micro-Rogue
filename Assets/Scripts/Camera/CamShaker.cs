using ManaSprite.SinusoidAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SinusoidAnimator))]
public class CamShaker : Singleton<CamShaker>
{
    public float decreaseFactor = 1.0f;
    SinusoidAnimator _sinusoidAnimator;

    Vector2 _currentShakeAmount;
    Vector2 _targetShakeAmount;
    Vector2 _directionOffset;

    private void Start()
    {
        _sinusoidAnimator = GetComponent<SinusoidAnimator>();
    }

    private void Update()
    {
        // Smoothly interpolate the current shake amount towards the target shake amount
        _currentShakeAmount = Vector2.Lerp(_currentShakeAmount, _targetShakeAmount, 2f * decreaseFactor * Time.deltaTime);

        // Apply the shake effect to the sinusoidAnimator
        _sinusoidAnimator.Amp = _currentShakeAmount.x;
        _sinusoidAnimator.Freq = 1f + _currentShakeAmount.y;

        // Decrease the target shake amount over time
        _targetShakeAmount = Vector2.MoveTowards(_targetShakeAmount, Vector2.zero, 0.5f * decreaseFactor * Time.deltaTime);
        CameraManager.I.Zoom(_targetShakeAmount.x.Remap(0f, 1f, 1f, 1.1f), this);

        // Directional Offset
        _directionOffset = _directionOffset.Lerp(Vector2.zero, 0.5f * decreaseFactor * Time.deltaTime);
        var camTransform = transform.GetChild(0);
        camTransform.localPosition = camTransform.localPosition.Lerp((Vector3)_directionOffset + Vector3.forward * camTransform.localPosition.z, decreaseFactor * Time.deltaTime);
    }

    public void Shake(float amplitude, float frequency, Vector2? direction = null)
    {
        // Hard Coded Mults
        amplitude *= 1f;
        frequency *= 1f * 12f;

        // Add the requested shake effect to the target shake amount
        float shakeAmp = 1f * amplitude;
        _targetShakeAmount += new Vector2(shakeAmp, frequency);
        _targetShakeAmount.x = Mathf.Clamp(_targetShakeAmount.x, 0f, 2f);
        _targetShakeAmount.y = Mathf.Clamp(_targetShakeAmount.y, 0f, 2f);

        _directionOffset += 5f * amplitude * direction?? Vector2.zero;

        //float ampPercent = amplitude.Remap(0.2f, 0.3f, 0f, 1f);
        //AmbientNotePlayer.I.PlayRiff(ampPercent.RemapPercent(0.5f, 2f));
    }
}