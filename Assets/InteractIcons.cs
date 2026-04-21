using System.Collections.Generic;
using UnityEngine;

public class InteractIcons : MonoBehaviour
{
    InteractionBubble _interactionBubble;
    List<Player> _playerList = new();

    private void Awake()
    {
        _interactionBubble = GetComponentInParent<InteractionBubble>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision, true);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleCollision(collision, false);
    }

    void HandleCollision(Collider2D col, bool isEntering)
    {
        var colPlayer = col.GetComponentInParent<Player>();
        if (colPlayer == null) return;

        if (isEntering && !_playerList.Contains(colPlayer)) _playerList.Add(colPlayer);
        else if (!isEntering && _playerList.Contains(colPlayer)) _playerList.Remove(colPlayer);
    }

    private void Update()
    {
        float targetS = _playerList.Count > 0? 1f : 0f;
        //if(_interactionBubble != null) targetS *= _interactionBubble.IsInteractable ? 1f : 0f;
        float s = transform.GetChild(0).localScale.x.Lerp(targetS, 6f * Time.deltaTime);
        transform.GetChild(0).localScale = new Vector3(s, s, 1f);
    }
}
