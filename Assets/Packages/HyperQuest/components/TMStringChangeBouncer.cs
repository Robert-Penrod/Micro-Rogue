using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMStringChangeBouncer : TMProSetter
{
    public float BounceMagnitude = 1.5f;
    public float BounceSpeed = 5f;
    public float BounceTime = 1f;
    float _bounceTick = float.MaxValue;
    public float ReturnSpeed = 3f;
    string _prevString;
    float _targetSize = 1f;
    float _initSize;
    public AudioClip BounceSound;
    public List<AudioClip> ChangeSound;
    bool _init = false;

    private void Start()
    {
        _prevString = TextMesh.text;
        _initSize = transform.localScale.x;
    }

    private void LateUpdate()
    {
        BounceCheck();
        BounceLerp();
        ReturnCheck();
        _bounceVolume = _bounceVolume.Lerp(0f, 5.5f * Time.deltaTime);
    }

    void ReturnCheck()
    {
        if (_bounceTick >= BounceTime)
        {
            _targetSize = _initSize;
            return;
        }
        _bounceTick += Time.deltaTime;
    }

    void BounceLerp()
    {
        // Get Target Scale
        Vector3 targetScale = _targetSize * Vector3.one;

        // Get Lerp Speed
        float currentSize = transform.localScale.x;
        float lerpSpeed = _targetSize > currentSize ? BounceSpeed : ReturnSpeed;

        // Lerp
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpSpeed * Time.deltaTime);
    }

    void BounceCheck()
    {
        if (_prevString != TextMesh.text)
        {
            DoBounce();
        }
        _prevString = TextMesh.text;
    }

    float _bounceVolume = 0f;

    void DoBounce()
    {
        //Debug.Log("Bounce");
        
        // Trigger bounce
        _bounceTick = 0f;
        _targetSize = _initSize * BounceMagnitude;

        // Audio
        AudioSpawner.PlayAudioWithRandPitch(BounceSound, 0.2f, 1f, (3.5f * _bounceVolume).ClampMax(0.4f));
        _bounceVolume += _bounceVolume.Remap(0f, 1f, 0.1f, 0f);

        AudioSpawner.PlayAudioWithRandPitch(ChangeSound.GetRandomElement(), 0.2f, 1f, 0.75f);
    }
}
