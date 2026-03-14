using ManaSprite.EasyPooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymbolPopup2DManager : PersistantSingleton<SymbolPopup2DManager>
{
    public GameObject TemplateObject;

    public SymbolPopup CreatePopup(Vector2 pos, string text = "", Sprite sprite = null, Color? c = null, Vector2? vel = null, Transform parent = null)
    {
        SymbolPopup symbolPopup = TemplateObject.PooledInstantiate().GetComponent<SymbolPopup>();
        symbolPopup.Velocity = vel ?? Vector2.zero;
        symbolPopup.transform.SetParent(parent);
        symbolPopup.TextMesh.text = text;
        symbolPopup.SpriteRend.sprite = sprite;
        if (c != null) symbolPopup.SpriteRend.color = c.Value;
        symbolPopup.transform.position = (Vector3)pos + Vector3.forward * TemplateObject.transform.position.z;
        symbolPopup.gameObject.SetActive(true);
        return symbolPopup;
    }
}
