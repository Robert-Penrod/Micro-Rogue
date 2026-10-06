using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour
{
    public float Zoom = 1f;
    public float Magnitude = 0.5f;
    public float LerpSpeed = 10f;
    Vector2 _targetPos;

    CameraManager _camManager;

    private void Start()
    {
        _camManager = CameraManager.I;
    }

    private void LateUpdate()
    {
        if (PlayerManager.I == null || _camManager == null) return;

        if(PlayerManager.I.AreAllPlayersDead())
        {
            if (DungeonManager.I.CombatEncounter != null)
            {
                var enemyList = DungeonManager.I.CombatEncounter.GetEnemies();
                if (enemyList.Count > 0)
                {
                    Vector2 avgPos = new Vector2();
                    enemyList.ForEach(enemy =>
                    {
                        avgPos += (Vector2)(enemy.transform.position);
                    });
                    avgPos /= enemyList.Count;
                    _targetPos = avgPos;
                }
            }
        }
        else if (PlayerManager.I.PlayerList.Count > 0)
        {
            Vector2 avgPos = new Vector2();
            PlayerManager.I.PlayerList.ForEach(player => 
            {
                if (player.Actor != null)
                {
                    avgPos += (Vector2)(player.Actor.transform.position);
                }
            });
            avgPos /= PlayerManager.I.PlayerList.Count;
            _targetPos = avgPos;

            // Zoom
            if(UpgradeMenu.I.IsOpen)
            {
                _targetPos = Vector2.zero;
            }
            else
            {
                _camManager.Zoom(Zoom, this);
            }
        }
        else
        {
            _targetPos = Vector2.zero;
            if (!UpgradeMenu.I?.IsOpen ?? false) _camManager.Zoom(0.9f, this);
        }
        _targetPos *= Magnitude;

        transform.position = Vector2.Lerp(transform.position, _targetPos, LerpSpeed * Time.deltaTime);
    }
}
