using HyperQuest.EasyPooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPopup2DManager : PersistantSingleton<TextPopup2DManager>
{
    public GameObject TemplateObject;

    public TextPopup CreatePopup(Vector2 pos, string text, Vector2? vel = null, Transform parent = null)
    {
        TextPopup textPopup = TemplateObject.PooledInstantiate().GetComponent<TextPopup>();
        textPopup.Velocity = vel ?? Vector2.zero;
        textPopup.transform.SetParent(parent);
        textPopup.TextMesh.text = text;
        textPopup.transform.position = (Vector3)pos + Vector3.forward * TemplateObject.transform.position.z;
        textPopup.gameObject.SetActive(true);
        return textPopup;
    }
}
