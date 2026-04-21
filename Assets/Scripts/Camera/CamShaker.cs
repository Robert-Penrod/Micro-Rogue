using ManaSprite.SinusoidAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SinusoidAnimator))]
public class CamShaker : Singleton<CamShaker>
{
    public float decreaseFactor = 1.0f;
    SinusoidAnimator _sinusoidAnimator;

    private Vector2 currentShakeAmount = Vector2.zero;
    private Vector2 targetShakeAmount = Vector2.zero;

    private void Start()
    {
        _sinusoidAnimator = GetComponent<SinusoidAnimator>();
    }

    private void Update()
    {
        // Smoothly interpolate the current shake amount towards the target shake amount
        currentShakeAmount = Vector2.Lerp(currentShakeAmount, targetShakeAmount, 2f * decreaseFactor * Time.deltaTime);

        // Apply the shake effect to the sinusoidAnimator
        _sinusoidAnimator.Amp = currentShakeAmount.x;
        _sinusoidAnimator.Freq = 1f + currentShakeAmount.y;

        // Decrease the target shake amount over time
        targetShakeAmount = Vector2.MoveTowards(targetShakeAmount, Vector2.zero, decreaseFactor * Time.deltaTime);
    }

    public void Shake(float amplitude, float frequency)
    {
        // Hard Coded Mults
        amplitude *= 1.5f;
        frequency *= 12f;

        // Add the requested shake effect to the target shake amount
        targetShakeAmount += new Vector2(amplitude, frequency);
        targetShakeAmount.x = Mathf.Clamp(targetShakeAmount.x, 0f, 1f);
        targetShakeAmount.y = Mathf.Clamp(targetShakeAmount.y, 0f, 2f);

        //float ampPercent = amplitude.Remap(0.2f, 0.3f, 0f, 1f);
        //AmbientNotePlayer.I.PlayRiff(ampPercent.RemapPercent(0.5f, 2f));
    }
}