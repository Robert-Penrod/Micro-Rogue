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
    TickTimer _lifeTimer = new TickTimer(1f); // 0.7
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
        _lifeTimer.Reset();
    }

    private void Update()
    {
        SpriteRend.enabled = SpriteRend.sprite != null;

        _lifeTimer.Tick(Time.deltaTime);
        float percent = _lifeTimer.GetPercent();
        if(percent == 1f)
        {
            gameObject.DestroyOrRecycle();
            return;
        }

        float sinPercent = Mathf.Sin(180f * percent * Mathf.Deg2Rad);
        transform.localScale = Vector3.one * sinPercent;

        transform.localPosition += 1.5f * sinPercent * Vector3.up * Time.deltaTime;
        transform.localPosition += (Vector3)Velocity * Time.deltaTime;

        TextMesh.color = TextMesh.color.Alpha(_alpha * sinPercent);
        SpriteRend.color = SpriteRend.color.Alpha(_initSpriteAlpha * sinPercent);

        Velocity *= (1f - _linearDamp * Time.deltaTime);
    }
}
