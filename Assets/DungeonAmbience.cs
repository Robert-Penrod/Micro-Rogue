using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DungeonAmbience : MonoBehaviour
{
    [SerializeField] AudioMixerGroup _mixerGroup;
    List<AudioClip> _dungeonAmbienceList => DungeonManager.I.GetBiomeData()._ambience;

    List<AudioSource> _audioSources = new();

    private void Start()
    {
        _audioSources.Clear();
        for(int i = 0; i < 1; i++)
        {
            _audioSources.Add(new GameObject("Ambience").AddComponent<AudioSource>());
            _audioSources[i].transform.SetParent(transform);
            _audioSources[i].playOnAwake = false;
            _audioSources[i].loop = true;
            _audioSources[i].outputAudioMixerGroup = _mixerGroup;
        }

        DungeonManager.I.OnGenerateLevel += () =>
        {
            _audioSources.ForEach(audioSource =>
            {
                Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
                audioSource.clip = _dungeonAmbienceList.GetRandomElement();
                audioSource.volume = 0.1f;
                audioSource.pitch = Random.Range(0.75f, 1f);
                audioSource.Play();
            });
        };
    }
}
