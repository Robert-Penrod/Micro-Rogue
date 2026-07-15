using UnityEngine;

public class PortalUpgradeInteractable : InteractableTrigger
{
    [SerializeField] Portal _portal;
    int upgradeLevel = 1;
    const int upgradeDelta = 5;
    int _upgradeCost => (int) Mathf.Pow(_portal.DungeonData.Coordinate.y, 2f);

    protected override void Interact(Player player)
    {
        Player.PlayerData.Gems -= _upgradeCost;
        _portal.DungeonData.Coordinate.y += 5;
        _portal.Level += 5;
    }

    protected override bool CanInteract(Player player)
    {
        return Player.PlayerData.Gems >= _upgradeCost;
    }
}
