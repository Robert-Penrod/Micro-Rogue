using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceLerper : MonoBehaviour
{
    public bool DoesPause = true;
    public bool UseScaledDeltaTime = false;
    public LerpValue Volume = new LerpValue(0f, 0f, 6f);
    public LerpValue Pitch = new LerpValue(0.9f, 0.9f, 6f);

    float _deltaTime => UseScaledDeltaTime ? Time.deltaTime : Time.unscaledDeltaTime;

    public static AudioSourceLerper Create(string name, Transform parent, AudioClip clip)
    {
        AudioSourceLerper audioSourceLerper = new GameObject(name).AddComponent<AudioSourceLerper>();
        audioSourceLerper.transform.SetParent(parent);
        audioSourceLerper.AudioSource.clip = clip;
        audioSourceLerper.AudioSource.Play();
        audioSourceLerper.Volume.SetValue(0f);
        audioSourceLerper.Pitch.SetValue(1f);
        return audioSourceLerper;
    }

    public AudioSource AudioSource
    {
        get
        {
            if(_audioSource == null) _audioSource = GetComponent<AudioSource>();
            return _audioSource;
        }
    }
    AudioSource _audioSource;

    private void Awake()
    {
        AudioSource.loop = true;
        AudioSource.volume = 0f;
    }

    private void Update()
    {
        Volume.Lerp(_deltaTime);
        Pitch.Lerp(_deltaTime);

        AudioSource.volume = Volume.Value;
        AudioSource.pitch = Pitch.Value;

        if(DoesPause) AudioSource.enabled = Time.timeScale > 0.01f;
    }
}
