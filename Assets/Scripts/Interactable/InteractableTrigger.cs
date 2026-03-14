using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractableTrigger : MonoBehaviour
{
    [SerializeField] float _interactTime = 1f;
    float _interactTick = 0f;
    List<Player> _interactingPlayerList = new();

    protected abstract void Interact(Player player);

    protected abstract bool CanInteract(Player player);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            var player = collision.GetComponentInParent<Player>();
            if(!_interactingPlayerList.Contains(player) && CanInteract(player))
            {
                _interactingPlayerList.Add(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponentInParent<Player>();
            if (_interactingPlayerList.Contains(player)) _interactingPlayerList.Remove(player);
        }
    }

    private void FixedUpdate()
    {
        if(_interactingPlayerList.Count == 0)
        {
            _interactTick = 0f;
        }
        else
        {
            _interactTick += Time.fixedDeltaTime;
            if(_interactTick >= _interactTime)
            {
                Interact(_interactingPlayerList[0]);
                if (!CanInteract(_interactingPlayerList[0])) _interactingPlayerList.RemoveAt(0);
                _interactTick = 0f;
            }
        }
    }
}
