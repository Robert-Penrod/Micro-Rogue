using ManaSprite.EasyPooling;
using System.Collections.Generic;
using UnityEngine;

public class Loot : Pickup, IPoolable
{
    public enum LootType { Coin = 0, Gem = 100}
    public LootType Type;
    public int Value = 1;
    [SerializeField] AudioClip _pickupSound;

    List<SpriteRenderer> _spriteRends = new();
    Dictionary<SpriteRenderer, float> _alphaDict = new();

    public float LifeTime = 3f;
    float _lifeTick;

    float _spawnTime = 0.25f;
    float _spawnTick;

    float _fadeTime = 0.25f;
    float _fadeTick;

    Rigidbody2D _body;

    bool _wasPickedUp = false;

    public void Initialize()
    {
        _lifeTick = _spawnTick = _fadeTick = 0f;
        _wasPickedUp = false;
    }

    private void Awake()
    {
        _spriteRends.AddRange(GetComponentsInChildren<SpriteRenderer>());
        _spriteRends.ForEach(x =>
        {
            _alphaDict.Add(x, x.color.a);
        });

        _body = GetComponent<Rigidbody2D>();
        Initialize();
    }

    private void Update()
    {

        if(_spawnTick < _spawnTime)
        {
            _spawnTick += Time.deltaTime;
            SetAlpha((_spawnTick / _spawnTime).Clamp01());
        }
        else if(_lifeTick < LifeTime)
        {
            _lifeTick += Time.deltaTime;
            SetAlpha((_lifeTick / LifeTime).RemapPercent(1f, 0.5f));
        }
        else if(_fadeTick < _fadeTime)
        {
            if (_wasPickedUp || (LifeTime > 0 && _body.linearVelocity.magnitude < 0.01f))
            {
                _fadeTick += Time.deltaTime;
            }

            if(_wasPickedUp)
            {
                transform.position += 2f * Vector3.up * Time.deltaTime;
            }
            SetAlpha((_fadeTick / _fadeTime).RemapPercent(0.5f, 0f));
        }
        else
        {
            this.gameObject.DestroyOrRecycle();
        }
    }

    void SetAlpha(float alpha)
    {
        _spriteRends.ForEach(x =>
        {
            x.color = x.color.Alpha(_alphaDict[x] * alpha);
        });
    }

    public override bool CanPickUp()
    {
        return !_wasPickedUp && _spawnTick >= _spawnTime && (_lifeTick < LifeTime || LifeTime < 0);
    }

    public override bool DoPickup(ActorPickupSystem pickupSystem)
    {
        if (!CanPickUp()) return false;

        var actor = pickupSystem.PickupActor;
        if (!actor.IsPlayer()) return false;

        var player = actor.GetComponentInParent<Player>();

        switch (Type)
        {
            case LootType.Coin:
                player.Data.Coin += Value;
                break;
            case LootType.Gem:
                Player.PlayerData.Gem += Value;
                break;
        }

        PlayAudio(_pickupSound);

        //Debug.Log("Looted!");
        _lifeTick = LifeTime; // End Life

        _body.linearVelocity *= 0.25f;

        _wasPickedUp = true;

        return true;
    }

    void PlayAudio(AudioClip clip)
    {
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.2f, 1f, 1f);
    }

    public override void IsGrabbing(float deltaTime)
    {
        float reverseAmount = 10f * deltaTime;
        if (_fadeTick > 0)
        {
            _fadeTick = (_fadeTick - reverseAmount).ClampMin(0);
        }
        else
        {
            _lifeTick = (_lifeTick - reverseAmount).ClampMin(0);
        }
    }
}
