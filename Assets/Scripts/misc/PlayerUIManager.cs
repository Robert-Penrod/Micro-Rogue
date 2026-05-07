using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    List<PlayerUI> _playerUIs = new();

    private void Awake()
    {
        // Init
        GetComponentsInChildren(_playerUIs);
        _playerUIs.ForEach(x =>
        {
            x.gameObject.SetActive(false);
        });

        // Player Join
        PlayerManager.I.OnPlayerJoin += (Player player) =>
        {
            PlayerDataChange();
        };
        // Player Leave
        PlayerManager.I.OnPlayerLeave += (Player player) =>
        {
            PlayerDataChange();
        };
    }

    void PlayerDataChange()
    {
        this.DelayedInvoke(-1, () =>
        {
            UpdatePlayerUIs();
        });
    }

    void UpdatePlayerUIs()
    {
        var playerList = PlayerManager.I.PlayerList;
        for(int i = 0; i < _playerUIs.Count; i++)
        {
            // Has Player
            if(i < playerList.Count)
            {
                _playerUIs[i].SetPlayer(playerList[i]);
                _playerUIs[i].gameObject.SetActive(true);
            }
            // No Player
            else
            {
                _playerUIs[i].gameObject.SetActive(false);
            }
        }
    }
}
