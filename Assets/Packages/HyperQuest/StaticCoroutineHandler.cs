using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutineHandler : MonoBehaviour
{
    public static StaticCoroutineHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                Instance = new GameObject("= Static_Coroutine_Handler =").AddComponent<StaticCoroutineHandler>();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    static StaticCoroutineHandler _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            Instance = (StaticCoroutineHandler)FindObjectOfType(typeof(StaticCoroutineHandler));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
