using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    public float Magnitude = 0.5f;
    public float LerpSpeed = 10f;
    Vector2 _targetPos;

    private void LateUpdate()
    {
        if (PlayerManager.I.PlayerList.Count > 0)
        {
            Vector2 avgPos = new Vector2();
            PlayerManager.I.PlayerList.ForEach(x => avgPos += (Vector2)(x.Actor.transform.position));
            avgPos /= PlayerManager.I.PlayerList.Count;
            _targetPos = avgPos;
        }
        else
        {
            _targetPos = Vector2.zero;
        }
        _targetPos *= Magnitude;

        transform.position = Vector2.Lerp(transform.position, _targetPos, LerpSpeed * Time.deltaTime);
    }
}
