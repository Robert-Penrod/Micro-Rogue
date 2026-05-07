using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortalLevelAltar_IBC : IBC
{
    int upgradeLevel = 1;
    [SerializeField] TextMeshPro _costText;

    private void Start()
    {
        _costText.text = "-" + GetUpgradeCost().ToString();
    }

    private void Update()
    {
        _interactionBubble.IsInteractable = Player.PlayerData.Gem >= GetUpgradeCost();
    }

    int GetUpgradeCost()
    {
        return (int)Mathf.Pow(2, upgradeLevel + 0);
    }

    public void DoUpgrade()
    {
        Player.PlayerData.Gem -= GetUpgradeCost();
        GetPortals().ForEach(portal =>
        {
            portal.DungeonData.Coordinate.y += 1;
            portal.UpdateUI();
        });
        upgradeLevel++;
        _costText.text = "-" + GetUpgradeCost().ToString();
    }

    List<Portal> GetPortals()
    {
        return new List<Portal>(FindObjectsByType<Portal>(FindObjectsSortMode.None));
    }
}
