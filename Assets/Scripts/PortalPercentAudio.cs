using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PortalPercentAudio : MonoBehaviour
{
    AudioSource _audioSource;
    DungeonManager _dungeonManager;
    float _lerpPercent;
    float _lerpPitch;
    float _basePitch;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _dungeonManager = DungeonManager.I;
        _dungeonManager.OnPortalTransitionStart += () =>
        {
            _basePitch = Random.Range(0.9f, 1.1f);
        };
    }

    private void Update()
    {
        // Vol
        _lerpPercent = _lerpPercent.Lerp(_dungeonManager.PortalPercent, 12f * Time.deltaTime);
        _audioSource.volume = _lerpPercent;

        // Pitch
        float targetPitch = _basePitch * _lerpPercent.RemapPercent(0.9f, 1f);
        _lerpPitch = _lerpPitch.Lerp(targetPitch, 12f * Time.deltaTime);
        _audioSource.pitch = _lerpPitch;
    }
}
