using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteSetter : MonoBehaviour
{
    [SerializeField] List<Sprite> _spriteList;
    [SerializeField] List<Color> _spriteColors;

    SpriteRenderer _spriteRend;
    Player _player;

    static float _randSeed = -1f;

    private void Awake()
    {
        _spriteRend = GetComponent<SpriteRenderer>();
        _player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        InitSprite();
    }

    void InitSprite()
    {
        // Seed
        if (_randSeed < 0)
        {
            _randSeed = ((100f * DateTime.Now.Ticks) % 100) / 100f;
        }

        // Sprite
        _spriteRend.sprite = _spriteList.GetRandomElement();

        // Color
        Debug.Log(_player.Index);
        _spriteRend.color = _spriteColors[(int)((_player.Index + _randSeed * _spriteColors.Count) % _spriteColors.Count)];
    }
}
