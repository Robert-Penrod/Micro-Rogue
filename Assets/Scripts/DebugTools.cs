using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                StartCoroutine(UpgradeManager.I.UpgradePlayers_Co());
            }
            else
            {
                UpgradeManager.I.FinishUpgrading();
            }
        }

        // Upgrade all NPCs
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Upgrading NPCs");
            new List<Actor>(FindObjectsByType<Actor>(FindObjectsSortMode.None)).ForEach(actor =>
            {
                if(!actor.IsPlayer())
                {
                    var upgradeOptions = UpgradeManager.I.GetUpgradeOptions(actor);
                    if (upgradeOptions.Count > 0) upgradeOptions[0].ApplyUpgrade();
                }
            });
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Restarting...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Deleting PlayerPrefs");
                PlayerPrefs.DeleteAll();
            }
        }

        // Slowmo Toggle
        if (Input.GetKeyDown(KeyCode.L))
        {
            _timeScaleIndex = (_timeScaleIndex + 1) % _timeScaleArray.Length;
            var timescale = _timeScaleArray[_timeScaleIndex];
            Debug.Log($"Timescale: {timescale}");
            Utils.SetFullTimeScale(timescale);
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
