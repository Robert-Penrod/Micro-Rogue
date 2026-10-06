using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("UI")]
    public AudioClip BtnHover;
    public AudioClip BtnSelect;
    public AudioClip BtnSubmit;

    public void Play(AudioClip clip)
    {
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.2f, 1f, 1f);
    }
}
