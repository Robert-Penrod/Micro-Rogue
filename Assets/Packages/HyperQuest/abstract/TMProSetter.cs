using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMProSetter : MonoBehaviour
{
    protected TextMeshProUGUI TextMesh;

    protected virtual void Awake()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
    }
}
