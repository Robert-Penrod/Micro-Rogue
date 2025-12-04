using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ManaSprite.EasyPooling;

[RequireComponent(typeof(TextMeshPro))]
public class TextPopup : MonoBehaviour, IPoolable
{
    [HideInInspector] public TextMeshPro TextMesh;
    TickTimer _lifeTimer = new TickTimer(0.7f);
    Rigidbody2D _rb;
    public Vector2 Velocity;

    float _linearDamp = 2f;
    float _alpha;

    private void Awake()
    {
        TextMesh = GetComponent<TextMeshPro>();
        _alpha = TextMesh.color.a;
        Initialize();
    }

    public void Initialize()
    {
        if(TextMesh == null) TextMesh = GetComponent<TextMeshPro>();
        _lifeTimer.Reset();
    }

    private void Update()
    {
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

        Velocity *= (1f - _linearDamp * Time.deltaTime);
    }
}
