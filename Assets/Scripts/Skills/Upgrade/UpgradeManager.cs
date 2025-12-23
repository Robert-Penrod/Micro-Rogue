using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : PersistantSingleton<UpgradeManager>
{
    public bool IsUpgrading { get; private set; }

    public void UpgradePlayers()
    {
        var playerList = PlayerManager.I.PlayerList;
        if(playerList.Count == 0)
        {
            Debug.Log("No players to upgrade");
            return;
        }


        IsUpgrading = true;
        Debug.Log("Upgrading Players!");
        UpgradeMenu.I.DoUpgradeMenuFor(playerList[0].Actor);        
    }

    public void CancelUpgrade()
    {
        IsUpgrading = false;
        UpgradeMenu.I.SetMenuOpen(false);
    }

    public List<Upgrade> GetUpgradeOptions(Actor actor)
    {
        return null; 
    }
}
