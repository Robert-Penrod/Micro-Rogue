using UnityEngine;

public class ActorBrain : MonoBehaviour
{
    protected Actor _actor;

    protected virtual void Awake()
    {
        _actor = GetComponent<Actor>();
    }
}
