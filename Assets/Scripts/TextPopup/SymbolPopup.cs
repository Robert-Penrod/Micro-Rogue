using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ManaSprite.EasyPooling;

[RequireComponent(typeof(TextMeshPro))]
public class SymbolPopup : MonoBehaviour, IPoolable
{
    [HideInInspector] public TextMeshPro TextMesh;
    public SpriteRenderer SpriteRend;
    float _lifeTick = 0f;
    public float LifeTime = 1f;
    public float Scale = 1f;
    float _lifePercent => _lifeTick / LifeTime;
    Rigidbody2D _rb;
    public Vector2 Velocity;

    float _linearDamp = 2f;
    float _alpha;
    float _initSpriteAlpha;

    private void Awake()
    {
        TextMesh = GetComponent<TextMeshPro>();
        SpriteRend = GetComponentInChildren<SpriteRenderer>();
        _alpha = TextMesh.color.a;
        _initSpriteAlpha = SpriteRend?.color.a ?? 0f;
        Initialize();
    }

    public void Initialize()
    {
        if(TextMesh == null) TextMesh = GetComponent<TextMeshPro>();
        if (SpriteRend == null) SpriteRend = GetComponentInChildren<SpriteRenderer>();
        _lifeTick = 0f;
        Scale = 1f;
        LifeTime = 1f;
    }

    private void Update()
    {
        SpriteRend.enabled = SpriteRend.sprite != null;

        _lifeTick += Time.deltaTime;
        if (_lifePercent >= 1f)
        {
            gameObject.DestroyOrRecycle();
            return;
        }

        float sinPercent = Mathf.Sin(180f * _lifePercent * Mathf.Deg2Rad);
        transform.localScale = Scale * Vector3.one * sinPercent;

        transform.localPosition += 1.5f * sinPercent * Vector3.up * Time.deltaTime;
        transform.localPosition += (Vector3)Velocity * Time.deltaTime;

        TextMesh.color = TextMesh.color.Alpha(_alpha * sinPercent);
        SpriteRend.color = SpriteRend.color.Alpha(_initSpriteAlpha * sinPercent);

        Velocity *= (1f - _linearDamp * Time.deltaTime);
    }
}
