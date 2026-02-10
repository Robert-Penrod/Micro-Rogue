using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    List<AudioClip> _hubMusicClips;
    List<AudioClip> _combatMusicClips;
    List<AudioClip> _upgradeMusicClips;
    List<AudioClip> _bossMusicClips;
    List<AudioClip> _eliteMusicClips;
    List<AudioClip> _gameOverMusicClips;
    AudioSourceLerper _hubMusic;
    AudioSourceLerper _combatMusic;
    AudioSourceLerper _upgradeMusic;
    AudioSourceLerper _bossMusic;
    AudioSourceLerper _eliteMusic;
    AudioSourceLerper _gameOverMusic;

    float _musicVol = 0.2f;

    List<AudioSourceLerper> _audioLerpers = new();

    private void Awake()
    {
        _hubMusicClips = new(Resources.LoadAll<AudioClip>("Music/Hub"));
        _combatMusicClips = new(Resources.LoadAll<AudioClip>("Music/Combat"));
        _upgradeMusicClips = new(Resources.LoadAll<AudioClip>("Music/Upgrade"));
        _bossMusicClips = new(Resources.LoadAll<AudioClip>("Music/Boss"));
        _eliteMusicClips = new(Resources.LoadAll<AudioClip>("Music/Elite"));
        _gameOverMusicClips = new(Resources.LoadAll<AudioClip>("Music/Game Over"));

        _hubMusic = AudioSourceLerper.Create("Hub Music", this.transform, _hubMusicClips);
        _combatMusic = AudioSourceLerper.Create("Combat Music", this.transform, _combatMusicClips);
        _upgradeMusic = AudioSourceLerper.Create("Upgrade Music", this.transform, _upgradeMusicClips);
        _bossMusic = AudioSourceLerper.Create("Boss Music", this.transform, _bossMusicClips);
        _eliteMusic = AudioSourceLerper.Create("Elite Music", this.transform, _eliteMusicClips);
        _gameOverMusic = AudioSourceLerper.Create("Game Over Music", this.transform, _gameOverMusicClips);
        _audioLerpers = new List<AudioSourceLerper> { _hubMusic, _combatMusic, _upgradeMusic, _bossMusic, _eliteMusic, _gameOverMusic };

        // Init 0 Volume
        _audioLerpers.ForEach(x =>
        {
            x.Volume.LerpMult = 0.75f;
            x.Volume.Value = 0f;
        });
    }

    private void Start()
    {
        // Random init start time
        _audioLerpers.ForEach(audioLerper =>
        {
            Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
            audioLerper.AudioSource.time = Random.Range(0f, 1f) * audioLerper.AudioSource.clip.length;
        });
    }

    private void Update()
    {
        // GameOver
        if(PlayerManager.I.AreAllPlayersDead())
        {
            PlayAudio(_gameOverMusic);
        }
        // Upgrading / Idle
        else if(UpgradeManager.I.IsUpgrading || (DungeonManager.I.IsEncounterOver && DungeonManager.I.Data.Coordinate.y != 0))
        {
            PlayAudio(_upgradeMusic);
        }
        // Hub
        else if(DungeonManager.I.Data.Coordinate.y == 0)
        {
            PlayAudio(_hubMusic);
        }
        // Combat
        else
        {
            // Sneak
            if(PlayerManager.I.PlayerList.FindAll(x => x.Actor.Senses.EnemyActors.Count > 0).Count == 0)
            {
                PlayAudio(_upgradeMusic);
            }
            // Boss
            else if (DungeonManager.I.Data.IsBoss)
            {
                PlayAudio(_bossMusic);
            }
            // Elite
            else if (DungeonManager.I.Data.IsElite)
            {

                PlayAudio(_eliteMusic);
            }
            // Combat
            else
            {
                PlayAudio(_combatMusic);
            }
        }
    }

    void PlayAudio(AudioSourceLerper playSource)
    {
        _hubMusic.Volume.Target = playSource == _hubMusic ? _musicVol : 0f;
        _combatMusic.Volume.Target = playSource == _combatMusic ? _musicVol : 0f;
        _upgradeMusic.Volume.Target = playSource == _upgradeMusic ? _musicVol : 0f;
        _bossMusic.Volume.Target = playSource == _bossMusic ? _musicVol : 0f;
        _eliteMusic.Volume.Target = playSource == _eliteMusic ? _musicVol : 0f;
        _gameOverMusic.Volume.Target = playSource == _gameOverMusic ? _musicVol : 0f;
    }
}
