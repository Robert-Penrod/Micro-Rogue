using UnityEngine;

public class CampUpgrade_IBC : IBC
{
    private void Update()
    {
        _interactionBubble.IsInteractable = Player.PlayerData.Gem >= CalculateCampUpgradeCost();
    }

    int CalculateCampUpgradeCost()
    {
        return 8;
    }
}
