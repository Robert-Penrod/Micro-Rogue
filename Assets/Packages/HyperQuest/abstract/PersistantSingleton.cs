using UnityEngine;

// <summary>
// Inherit from this base class to create a singleton that isn't destroyed on load.
// e.g. public class MyClassName : Singleton<MyClassName> {}
// </summary>
public abstract class PersistantSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T I { get; private set; }

    protected virtual void Awake()
    {
        transform.SetParent(null);

        if (I == null)
        {
            I = (T)FindObjectOfType(typeof(T));
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

