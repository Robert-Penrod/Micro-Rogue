using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    #region Classes
    [System.Serializable]
    public class AudioData
    {
        public AudioClip Clip;
        public float Volume = 1f;
        public float Pitch = 1f;
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceEventer : MonoBehaviour
    {
        AudioSource _audioSource;
        bool _wasPlaying = false;

        public Action<AudioSource> OnStartPlaying;
        public Action<AudioSource> OnStopPlaying;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            // Started playing
            if (_audioSource.isPlaying && !_wasPlaying)
            {
                OnStartPlaying?.Invoke(_audioSource);
                _wasPlaying = true;
            }

            // Stopped playing
            if (!_audioSource.isPlaying && _wasPlaying)
            {
                OnStopPlaying?.Invoke(_audioSource);
                _wasPlaying = false;
            }
        }
    }
    #endregion

public static class AudioSpawner
{
    private const bool HIDE_AUDIO_IN_HEIRARCHY = true;
    private const float DEFAULT_MIN_DIST = 10f;
    private const float DEFAULT_MAX_DIST = 50f;

    // Pooling
    static List<AudioSource> _audioSourcePool = new List<AudioSource>();
    static List<AudioSource> _activeAudioSourceList = new List<AudioSource>();
    static int _audioSourceCount;
    private const int MAX_POOL_COUNT = 1000;

    public static void InitPool(int count = MAX_POOL_COUNT)
    {
        ClearPool();
        for(int i = 0; i < count; i++)
        {
            CreateNewPooledAudioSource();
        }
    }

    static void ClearPool()
    {
        _audioSourcePool.ForEach(x => MonoBehaviour.Destroy(x.gameObject));
        _audioSourcePool.Clear();
        _activeAudioSourceList.ForEach(x => MonoBehaviour.Destroy(x.gameObject));
        _activeAudioSourceList.Clear();
    }

    static AudioSource CreateNewPooledAudioSource()
    {
        GameObject audioObject = new GameObject();
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        if (HIDE_AUDIO_IN_HEIRARCHY) audioSource.gameObject.hideFlags = HideFlags.HideInHierarchy;

        audioSource.priority = 200;

        // Setup pooling logic
        _audioSourcePool.Add(audioSource);
        AudioSourceEventer audioEventer = audioObject.AddComponent<AudioSourceEventer>();
        audioEventer.OnStartPlaying += (audioSource) =>
        {
            _audioSourcePool.Remove(audioSource);
            _activeAudioSourceList.Add(audioSource);
        };
        audioEventer.OnStopPlaying += (audioSource) =>
        {
            _activeAudioSourceList.Remove(audioSource);
            _audioSourcePool.Add(audioSource);
        };

        return audioSource;
    }
    
    static AudioSource GetAudioSource()
    {
        GameObject audioObject = new GameObject();
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        if (HIDE_AUDIO_IN_HEIRARCHY) audioSource.gameObject.hideFlags = HideFlags.HideInHierarchy;
        audioObject.AddComponent<AudioSourceAutoDestroy>();
        return audioSource;
        /*
        // Remove nulls
        _audioSourcePool.RemoveAll(x => x == null);
        _activeAudioSourceList.RemoveAll(x => x == null);

        if(_audioSourcePool.Count > 0)
        {
            AudioSource audioSource = _audioSourcePool[0];
            audioSource.Stop();
            return audioSource;
        }
        else
        {
            // If we've hit the pool limit, stop and reuse an active audio source
            if (_audioSourceCount >= MAX_POOL_COUNT)
            {
                AudioSource oldestPlayingAudioSource = _activeAudioSourceList[_activeAudioSourceList.Count - 1];
                oldestPlayingAudioSource.Stop();
                return oldestPlayingAudioSource;
            }
            // Otherwise just spawn a new pooled audio source
            else
            {
                return CreateNewPooledAudioSource();
            }
        }
        */
    }

    public static AudioSource PlayAudioWithRandPitch(AudioClip audioClip, float pitchRange = 0.2f, float basePitch = 1f, float volume = 1f, Vector3? position = null, float delay = 0f) => PlayAudio(audioClip, volume, basePitch + UnityEngine.Random.Range(-1f, 1f) * pitchRange, position, delay: delay);

    public static AudioSource PlayAudio(AudioData audioData) => PlayAudio(audioData.Clip, audioData.Volume, audioData.Pitch, null);
    public static AudioSource PlayAudio(AudioClip audioClip, float volume = 1f, float pitch = 1f) => PlayAudio(audioClip, volume, pitch, null);
    public static AudioSource PlayAudio(AudioData audioData, Vector3? position = null, float minDistance = DEFAULT_MIN_DIST, float maxDistance = DEFAULT_MAX_DIST) => PlayAudio(audioData.Clip, audioData.Volume, audioData.Pitch, position, minDistance, maxDistance);
    public static AudioSource PlayAudio(AudioClip audioClip, float volume, float pitch, Vector3? position = null, float minDistance = DEFAULT_MIN_DIST, float maxDistance = DEFAULT_MAX_DIST, float delay = 0f)
    {
        // Get Audio Source
        AudioSource audioSource = GetAudioSource();
        //AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
        //audioSource.gameObject.AddComponent<DelayedDestroyer>().DestroyTime = 5f;

        // Setup
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        // Spatial Audio
        if (position != null)
        {
            audioSource.spatialBlend = 0.5f;
            audioSource.transform.position = position.Value;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
        }
        else
        {
            audioSource.spatialBlend = 0f;
        }

        // Play call
        if (audioClip != null)
        {
            if(delay > 0)
            {
                audioSource.clip = audioClip;
                audioSource.PlayDelayed(delay);
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

        // Done
        return audioSource;
    }
   
}


[RequireComponent(typeof(AudioSource))]
public class AudioSourceAutoDestroy : MonoBehaviour
{
    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
