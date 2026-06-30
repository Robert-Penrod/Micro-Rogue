using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTools : MonoBehaviour
{
    int _timeScaleIndex = 0;
    float[] _timeScaleArray = { 1, 0.5f, 0.2f, 0.1f };

    private void Update()
    {
        // Gold
        if(Input.GetKeyDown(KeyCode.G))
        {
            PlayerManager.I.PlayerList.ForEach(player =>
            {
                player.Data.Coin += 5;
            });
        }

        // Deep Rest
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            PlayerManager.I.PlayerList.ForEach(player =>
            {
                player.Actor.DeepRest();
            });
        }

        // Player Damage
        if(Input.GetKeyDown(KeyCode.LeftBracket))
        {
            PlayerManager.I.PlayerList.ForEach(player =>
            {
                player.Actor.TakeDamage(Random.Range(1, 4), null, null);
            });
        }

        // Gems
        if(Input.GetKeyDown(KeyCode.H))
        {
            Player.PlayerData.Gem += 5;
        }

        // Upgrade
        if(Input.GetKeyDown(KeyCode.U))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                DungeonManager.I.PreviousData = DungeonManager.I.Data;
                DungeonManager.I.PreviousData.EliteTier = 1;
            }
            else
            {
                DungeonManager.I.PreviousData = DungeonManager.I.Data;
                DungeonManager.I.PreviousData.EliteTier = 0;
            }

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
        if(Input.GetKey(KeyCode.Equals))
        {
            bool upgradePlayer = Input.GetKeyDown(KeyCode.P);
            bool upgradeEnemy = Input.GetKeyDown(KeyCode.E);
            if (upgradePlayer || upgradeEnemy)
            {
                Debug.Log($"Upgrading {(upgradePlayer? "allys" : string.Empty)} {(upgradeEnemy? "enemies" : string.Empty)}");
                new List<Actor>(FindObjectsByType<Actor>(FindObjectsSortMode.None)).ForEach(actor =>
                {
                    if (!actor.IsPlayer())
                    {
                        if ((actor.Faction == Actor.FactionType.Player && upgradePlayer) || (actor.Faction == Actor.FactionType.Enemy && upgradeEnemy))
                        {
                            var upgradeOptions = UpgradeManager.I.GetUpgradeOptions(actor);
                            if (upgradeOptions.Count > 0) upgradeOptions[0].ApplyUpgrade();
                        }
                    }
                });
            }
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
        if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            Utils.SetDefaultTimeScale(2f);
        }
        else if(Input.GetKeyUp(KeyCode.Semicolon))
        {
            Utils.SetFullTimeScale(_timeScaleArray[_timeScaleIndex]);
        }

        // Kill
        if (Input.GetKeyDown(KeyCode.K))
        {
            var actorList = new List<Actor>(FindObjectsByType<Actor>(FindObjectsSortMode.None));
            actorList.ForEach(x =>
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (x.Faction != Actor.FactionType.Player) return;
                    x.Stats.Health = 0;
                }
                else
                {
                    if (x.Faction == Actor.FactionType.Player) return;
                    x.Stats.Health = 0;
                }
            });
        }


        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E Rand: " + Random.Range(1, 4));
        }
    }
}
