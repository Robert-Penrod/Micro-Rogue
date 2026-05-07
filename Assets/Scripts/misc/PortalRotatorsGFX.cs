using System.Collections.Generic;
using UnityEngine;

public class PortalRotatorsGFX : MonoBehaviour
{
    Portal _portal;

    List<SpriteRenderer> _spriteList = new();
    List<Rotator> _rotatorList = new();
    List<ParticleSystem> _pSystemList = new();
    List<Color> _initSpriteColors = new();
    List<float> _initRotSpeeds = new();


    private void Awake()
    {
        _portal = GetComponentInParent<Portal>();

        // SpriteRends
        _spriteList = new(GetComponentsInChildren<SpriteRenderer>());
        _initSpriteColors = new();
        for(int i = 0; i < _spriteList.Count; i++)
        {
            _initSpriteColors.Add(_spriteList[i].color);
        }

        // Rotators
        _rotatorList = new(GetComponentsInChildren<Rotator>());
        _initRotSpeeds = new();
        for(int i = 0; i < _rotatorList.Count; i++)
        {
            _initRotSpeeds.Add(_rotatorList[i].Speed);
        }

        // Particles
        _pSystemList = new(GetComponentsInChildren<ParticleSystem>());
    }

    private void Update()
    {
        // Rotation
        float rotMult = _portal.IsEnterable ? 1f : 0.1f;
        for(int i = 0; i < _rotatorList.Count; i++)
        {
            _rotatorList[i].Speed = _initRotSpeeds[i] * rotMult;
        }

        // Color
        Color nonEnterableColor = Color.white;
        float colorLerp = 0.5f;
        float alphaMult = _portal.IsEnterable ? 1f : 0.75f;
        for(int i = 0; i < _spriteList.Count; i++)
        {
            _spriteList[i].color = _portal.IsEnterable ? _initSpriteColors[i] : _initSpriteColors[i].Lerp(nonEnterableColor, colorLerp).Alpha(_initSpriteColors[i].a * alphaMult);
        }

        // Particles
        _pSystemList.ForEach(pSystem =>
        {
            if (_portal.IsEnterable && !pSystem.isPlaying) pSystem.Play();
            else if (!_portal.IsEnterable && pSystem.isPlaying) pSystem.Stop();
        });
    }
}
