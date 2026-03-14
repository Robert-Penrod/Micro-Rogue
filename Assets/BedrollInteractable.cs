using TMPro;
using UnityEngine;

public class BedrollInteractable : InteractableTrigger
{
    [SerializeField] int _gemCost = 25;
    [SerializeField] TextMeshPro _tmPro;

    private void OnValidate()
    {
        _tmPro.text = _gemCost.ToString();
    }

    protected override bool CanInteract(Player player)
    {
        return (Player.PlayerData.Gem >= _gemCost) && player.Actor.Stats.HealthPercent != 2f;
    }

    protected override void Interact(Player player)
    {
        Player.PlayerData.Gem -= _gemCost;
        PlayerManager.I.PlayerList.ForEach(player =>
        {
            player.Actor.DeepRest();
        });
    }
}
