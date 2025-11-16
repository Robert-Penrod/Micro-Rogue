using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteSetter : MonoBehaviour
{
    [SerializeField] List<Sprite> _spriteList;

    SpriteRenderer _spriteRend;
    Player _player;

    static int _colorOffset = -1;

    private void Awake()
    {
        _spriteRend = GetComponent<SpriteRenderer>();
        _player = GetComponentInParent<Player>();

        Random.InitState(DateTime.Now.GetHashCode());
        if (_colorOffset < 0) _colorOffset = Random.Range(0, _spriteList.Count);
        _colorOffset++;
    }

    private void Start()
    {
        InitSprite();
    }

    void InitSprite()
    {
        Random.InitState(DateTime.Now.GetHashCode());

        // Sprite
        _spriteRend.sprite = _spriteList.GetRandomElement();

        // Color
        _spriteRend.color = _player.Data.Color;
    }
}
