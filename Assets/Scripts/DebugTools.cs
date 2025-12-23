using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    int _timeScaleIndex = 0;
    float[] _timeScaleArray = { 1, 0.5f, 0.2f, 0.1f };

    private void Update()
    {
        // Upgrade
        if(Input.GetKeyDown(KeyCode.U))
        {
            if(!UpgradeManager.I.IsUpgrading)
            {
                UpgradeManager.I.UpgradePlayers();
            }
            else
            {
                UpgradeManager.I.CancelUpgrade();
            }
        }

        // Slowmo Toggle
        if (Input.GetKeyDown(KeyCode.L))
        {
            _timeScaleIndex = (_timeScaleIndex + 1) % _timeScaleArray.Length;
            Utils.SetFullTimeScale(_timeScaleArray[_timeScaleIndex]);
        }

        // Kill
        if (Input.GetKeyDown(KeyCode.K))
        {
            var actorList = new List<Actor>(FindObjectsByType<Actor>(FindObjectsSortMode.None));
            actorList.ForEach(x =>
            {
                if (x.Faction == Actor.FactionType.Player) return;
                x.Stats.Health = 0;
            });
        }
    }
}
