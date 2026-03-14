using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventer : MonoBehaviour
{
    public UnityEvent OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke();
    }
}
