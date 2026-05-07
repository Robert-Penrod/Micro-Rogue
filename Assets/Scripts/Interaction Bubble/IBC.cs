using UnityEngine;

[RequireComponent(typeof(InteractionBubble))]
public class IBC : MonoBehaviour
{
    protected InteractionBubble _interactionBubble;

    protected virtual void Awake()
    {
        _interactionBubble = GetComponent<InteractionBubble>();
    }
}
