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
                    // Open Menu
                    SetOpen(true);

                    // Gold -> Gem Conversion
                    PlayerManager.I.PlayerList.ForEach(player =>
                    {
                        Player.PlayerData.Gems += player.Data.Gold / 20;
                        player.Data.Gold = 0;
                    });

                    // Shop Restock
                    string shopRestockKey = "ShopRestockTick";
                    int shopRestockTick = PlayerPrefs.GetInt(shopRestockKey, 0);
                    shopRestockTick += DungeonManager.I.Data.Coordinate.y;
                    int restockCount = shopRestockTick / 4;
                    shopRestockTick -= restockCount * 4;
                    PlayerPrefs.SetInt(shopRestockKey, shopRestockTick);
                    var shopMenu = FindFirstObjectByType<ShopMenu>();
                    shopMenu.GenerateInventory(restockCount);

                    //Debug.Log($"Restock Tick {shopRestockTick}");
                    //Debug.Log($"Restock Count {restockCount}");
                }
            });
        }
    }
}
