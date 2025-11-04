using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ScriptableObjectDataWrapper<T> : ScriptableObject
{
    public T Data;
}
