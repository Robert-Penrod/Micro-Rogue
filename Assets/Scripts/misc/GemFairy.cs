using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemFairy : MonoBehaviour
{
    [SerializeField] Transform _gemGun;
    [SerializeField] GameObject _gemPrefab;
    [SerializeField] AudioClip _audio;
    [SerializeField] Transform _avatar;
    bool _needsToDropGems => PlayerManager.I.PlayerList.Count > 0 && _potentialGemCount < _cost && HubRoom.I.Level == 0;
    int _potentialGemCount => Player.PlayerData.Gem + _droppedGems.FindAll(x => x != null).Count;
    float _timer;
    [SerializeField] float _activeTime = 5f;
    bool _isActive => _timer >= _activeTime;
    List<GameObject> _droppedGems = new();
    int _cost = 2;
    float _spawnGemTick;
    [SerializeField] float _spawnGemTime = 0.25f;
    [SerializeField] float _spread = 0.125f;

    float _goodbyeTick;

    private void Start()
    {
        _avatar.transform.localScale = Vector3.zero;
        _avatar.gameObject.SetActive(true);
        _goodbyeTick = float.MaxValue;
    }

    private void Update()
    {
        if(_isActive)
        {
            _goodbyeTick = 0f;
        }
        else
        {
            _goodbyeTick += Time.deltaTime;
        }

        // Scale
        float targetScale = _isActive ? 1f : (_goodbyeTick > 1f? 0f : 1f);
        float lerpScale = _avatar.transform.localScale.x.Lerp(targetScale, 6f * Time.deltaTime);
        _avatar.transform.localScale = new Vector3(lerpScale, lerpScale, 1f);

        // Timer
        if(_needsToDropGems)
        {
            float prevTimer = _timer;
            _timer += Time.deltaTime;
            if(_timer >= _activeTime && prevTimer < _activeTime)
            {
                // Became Active
                PlayAudio();
            }
        }
        else
        {
            _timer = 0f;
        }

        // Gem Drop
        if(_isActive)
        {
            int gemsRemaining = _cost - (_droppedGems.Count + Player.PlayerData.Gem);
            if(gemsRemaining > 0)
            {
                _spawnGemTick += Time.deltaTime;
                if(_spawnGemTick >= _spawnGemTime)
                {
                    _spawnGemTick -= _spawnGemTime;
                    SpawnGem();
                }
            }
        }
        else
        {
            _spawnGemTick = 0f;
        }
    }

    void SpawnGem()
    {
        var gemObj = Instantiate(_gemPrefab, transform.position, Quaternion.identity);
        var gemBody = gemObj.GetComponent<Rigidbody2D>();
        if(gemBody != null)
        {
            Vector2 gemDir = _gemGun.up;
            gemDir = gemDir.RotationalLerp(Random.insideUnitCircle, _spread);
            Vector2 gemForce = 2.5f * gemDir;
            gemForce *= Random.Range(0.9f, 1.1f);
            gemBody.AddDampForce(gemForce, ForceMode2D.Impulse);
        }
        _droppedGems.Add(gemObj);

        PlayAudio(0.5f);
    }

    void PlayAudio(float mult = 1f)
    {
        AudioSpawner.PlayAudioWithRandPitch(_audio, 0.2f, 1f, mult);
    }
}
