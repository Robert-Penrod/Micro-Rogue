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

    CombatEncounterObject _combatEncounterObj;

    private void Start()
    {
        _camManager = CameraManager.I;
        
    }

    private void LateUpdate()
    {
        if (PlayerManager.I == null) return;

        if (PlayerManager.I.PlayerList.Count > 0)
        {
            Vector2 avgPos = new Vector2();
            PlayerManager.I.PlayerList.ForEach(player => avgPos += (Vector2)(player.Actor.transform.position));
            avgPos /= PlayerManager.I.PlayerList.Count;
            _targetPos = avgPos;

            // Zoom
            if(UpgradeMenu.I.IsOpen)
            {
                _targetPos = Vector2.zero;
            }
            else
            {
                _camManager.ZoomKnob = Zoom;
            }
        }
        else
        {
            _targetPos = Vector2.zero;
            if (!UpgradeMenu.I.IsOpen) _camManager.ZoomKnob = 1f;
        }
        _targetPos *= Magnitude;

        transform.position = Vector2.Lerp(transform.position, _targetPos, LerpSpeed * Time.deltaTime);
    }
}
