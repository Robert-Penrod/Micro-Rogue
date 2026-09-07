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

    float _fadeTime = 0.3f;
    float _fadeTick;

    Rigidbody2D _body;

    bool _wasPickedUp = false;

    public void Initialize()
    {
        _lifeTick = _spawnTick = _fadeTick = 0f;
        _wasPickedUp = false;
        this.gameObject.SetCollidersEnabled2D(true);
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
        float targetAlpha = 1f;
        float targetScale = 1f;
        float lerpMult = 1f;

        if(_spawnTick < _spawnTime)
        {
            _spawnTick += Time.deltaTime;
            float spawnPercent = (_spawnTick / _spawnTime).Clamp01();
            targetAlpha = (_spawnTick / _spawnTime).Clamp01();
            targetScale = spawnPercent.RemapPercent(0.5f, 1f);
            lerpMult = 10f;
        }
        else if(_lifeTick < LifeTime)
        {
            _lifeTick += Time.deltaTime;
            float lifePercent = _lifeTick / LifeTime;
            targetAlpha = lifePercent.RemapPercent(1f, 0.5f);
            targetScale = lifePercent.RemapPercent(1f, 0.675f);
        }
        else if(_fadeTick < _fadeTime)
        {
            if (_wasPickedUp || (LifeTime > 0 && _body.linearVelocity.magnitude < 0.01f))
            {
                _fadeTick += Time.deltaTime;
            }

            if(_wasPickedUp)
            {
                transform.position += 1f * Vector3.up * Time.deltaTime;
                targetScale = transform.localScale.x.Lerp(1f, 3f * Time.deltaTime);
                this.gameObject.SetCollidersEnabled2D(false);
            }

            if (LifeTime > 0)
            {
                float fadePercent = _fadeTick / _fadeTime;
                targetAlpha = fadePercent.RemapPercent(0.5f, 0f);
                targetScale = fadePercent.RemapPercent(0.675f, 0.5f);
                lerpMult = fadePercent.RemapPercent(2f, 100f);
            }
        }
        else
        {
            this.gameObject.DestroyOrRecycle();
        }

        float lerpSpeed = 12f;
        transform.localScale = Vector3.one * transform.localScale.x.Lerp(targetScale, lerpMult * lerpSpeed * Time.deltaTime);
        LerpAlpha(targetAlpha, lerpSpeed * Time.deltaTime);
    }

    void SetAlpha(float alpha)
    {
        _spriteRends.ForEach(x =>
        {
            x.color = x.color.Alpha(_alphaDict[x] * alpha);
        });
    }

    void LerpAlpha(float alpha, float deltaT)
    {
        _spriteRends.ForEach(x =>
        {
            x.color = x.color.Alpha(x.color.a.Lerp(_alphaDict[x] * alpha, deltaT));
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
                player.Data.Gold += Value;
                break;
            case LootType.Gem:
                Player.PlayerData.Gems += Value;
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
