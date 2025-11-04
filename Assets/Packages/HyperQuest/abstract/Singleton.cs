using UnityEngine;

// <summary>
// Inherit from this base class to create a singleton.
// e.g. public class MyClassName : Singleton<MyClassName> {}
// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)FindObjectOfType(typeof(T));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

