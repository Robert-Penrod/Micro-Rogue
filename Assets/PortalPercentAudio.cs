using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PortalPercentAudio : MonoBehaviour
{
    AudioSource _audioSource;
    DungeonManager _dungeonManager;
    float _lerpPercent;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _dungeonManager = DungeonManager.I;
    }

    private void Update()
    {
        _lerpPercent = _lerpPercent.Lerp(_dungeonManager.PortalPercent, 12f * Time.deltaTime);
        _audioSource.volume = _lerpPercent;
        _audioSource.pitch = _lerpPercent.RemapPercent(0.9f, 1f);
    }
}
