using System.Collections.Generic;
using UnityEngine;

public class ActorSFX : MonoBehaviour
{
    [SerializeField] List<AudioClip> _actorClip;
    Actor _actor;
    SimpleNPCBrain _npcBrain;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _npcBrain = GetComponentInParent<SimpleNPCBrain>();
    }

    private void OnEnable()
    {
        if (gameObject.tag.Equals("Player"))
        {
            PlayAudio(0.5f);
        }
    }

    private void Start()
    {
        if (_npcBrain != null)
        {
            _npcBrain.OnNotice += () =>
            {
                PlayAudio();
            };

            _npcBrain.OnLostTrail += () =>
            {
                PlayAudio(0.75f);
            };
        }

        _actor.OnDeath += () =>
        {
            PlayAudio(0.75f, 0.75f);
        };

        _actor.OnTakeDamage += () =>
        {
            if (Random.value < 0.25f)
            {
                PlayAudio(0.75f, 1f);
            }
        };
    }

    void PlayAudio(float intensity = 1f, float pitchMult = 1f)
    {
        //Random.InitState(_actor.name.Substring(0, 3).GetHashCode());
        Random.InitState(_actor.GetInstanceID().GetHashCode() + 3);
        var clip = _actorClip.GetRandomElement();
        Utils.RandomSeed();
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.15f, pitchMult, intensity * 0.5f, transform.position);
    }
}
