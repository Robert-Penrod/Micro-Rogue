using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioEntry
{
    public AudioClip AudioClip = null;
    public float Volume = 1f;
    public float Pitch = 1f;
    public float RandomPitch = 0f;

    public void Play()
    {
        AudioSpawner.PlayAudioWithRandPitch(AudioClip, RandomPitch, Pitch, Volume);
    }
}
