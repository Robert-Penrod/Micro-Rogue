using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip _hubMusicClip;
    [SerializeField] AudioClip _combatMusicClip;
    [SerializeField] AudioClip _upgradeMusicClip;
    [SerializeField] AudioClip _bossMusicClip;
    [SerializeField] AudioClip _eliteMusicClip;
    [SerializeField] AudioClip _gameOverMusicClip;
    AudioSourceLerper _hubMusic;
    AudioSourceLerper _combatMusic;
    AudioSourceLerper _upgradeMusic;
    AudioSourceLerper _bossMusic;
    AudioSourceLerper _eliteMusic;
    AudioSourceLerper _gameOverMusic;

    float _musicVol = 0.2f;

    private void Awake()
    {
        _hubMusic = AudioSourceLerper.Create("Hub Music", this.transform, _hubMusicClip);
        _combatMusic = AudioSourceLerper.Create("Combat Music", this.transform, _combatMusicClip);
        _upgradeMusic = AudioSourceLerper.Create("Upgrade Music", this.transform, _upgradeMusicClip);
        _bossMusic = AudioSourceLerper.Create("Boss Music", this.transform, _bossMusicClip);
        _eliteMusic = AudioSourceLerper.Create("Elite Music", this.transform, _eliteMusicClip);
        _gameOverMusic = AudioSourceLerper.Create("Game Over Music", this.transform, _gameOverMusicClip);

        _gameOverMusic.Volume.LerpMult = _hubMusic.Volume.LerpMult = _combatMusic.Volume.LerpMult = _upgradeMusic.Volume.LerpMult = _bossMusic.Volume.LerpMult = _eliteMusic.Volume.LerpMult = 0.7f;
        _gameOverMusic.Volume.Target = _gameOverMusic.Volume.Value = _hubMusic.Volume.Target = _hubMusic.Volume.Value = _combatMusic.Volume.Target = _combatMusic.Volume.Value = _upgradeMusic.Volume.Target = _upgradeMusic.Volume.Value = 0f;
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
            if (DungeonManager.I.Data.IsBoss)
            {
                PlayAudio(_bossMusic);
            }
            else if (DungeonManager.I.Data.IsElite)
            {

                PlayAudio(_eliteMusic);
            }
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
