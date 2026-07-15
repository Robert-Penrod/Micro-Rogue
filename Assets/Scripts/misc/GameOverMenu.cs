using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : SimpleMenu
{
    bool _isGameOver;

    protected override void Update()
    {
        base.Update();

        if (!_isGameOver && PlayerManager.I.AreAllPlayersDead())
        {
            _isGameOver = true;
            this.DelayedInvoke(1f, () =>
            {
                if (!IsOpen)
                {
                    PlayerManager.I.PlayerList.ForEach(player =>
                    {
                        Player.PlayerData.Gems += player.Data.Gold / 20;
                        player.Data.Gold = 0;
                    });
                    SetOpen(true);
                }
            });
        }
    }
}
