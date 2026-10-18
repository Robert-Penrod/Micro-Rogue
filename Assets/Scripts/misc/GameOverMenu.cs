using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : SimpleMenu
{
    [SerializeField] TextMeshProUGUI _infoLeft;
    [SerializeField] TextMeshProUGUI _infoRight;
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

                    UpdateInfo();

                    // Gold -> Gem Conversion
                    PlayerManager.I.PlayerList.ForEach(player =>
                    {
                        Player.PlayerData.Gems += player.Data.Gold / 20;
                        player.Data.Gold = 0;
                    });
                }
            });
        }
    }

    void UpdateInfo()
    {
        // Init
        string labelStrings = string.Empty;
        string valueStrings = string.Empty;

        // Get Info
        int floor = DungeonManager.I.Data.Coordinate.y;
        int gold = PlayerManager.I.PlayerList[0].Data.Gold;
        int gems = DungeonManager.I.RunInfoData.GemsCollected;
        float runTime = DungeonManager.I.RunInfoData.RunTime;
        float damageDone = 0;
        int killCount = 0;
        PlayerManager.I.PlayerList.ForEach(player =>
        {
            player.Actor.SkillSystem.SkillList.ForEach(skill =>
            {
                killCount += skill.MetaSkillInfo.Kills;
                damageDone += skill.MetaSkillInfo.DamageDone;
            });
        });

        int score = (floor * gold.ClampMin(1) * gems.ClampMin(1));

        // Strings
        labelStrings += "Floor:\n";
        valueStrings += $"{floor}\n";

        labelStrings += "Gold:\n";
        valueStrings += $"{gold}\n";

        labelStrings += "Gems:\n";
        valueStrings += $"{gems}\n";

        labelStrings += "Kills:\n";
        valueStrings += $"{killCount}\n";

        labelStrings += "Damage:\n";
        valueStrings += $"{(int)damageDone}\n";

        labelStrings += "Run Time:\n";
        valueStrings += $"{runTime.SecondsToMonoTimeStringMinSec(_infoRight.fontSize)}\n";

        labelStrings += "Score:";
        valueStrings += $"{score}";

        // Write
        _infoLeft.text = labelStrings;
        _infoRight.text = valueStrings;
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("Restarting", 1);
        RestartScene();
    }
}
