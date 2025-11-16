using Unity.Cinemachine;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    [SerializeField] Transform _camTarget;
    [SerializeField] float _lerpSpeed = 3f;
    [SerializeField] float _centerOffsetMult = 0.2f;
    PlayerManager _playerManager;
    CinemachineCamera _cinemachineCamera;

    

    private void Start()
    {
        _playerManager = PlayerManager.I;
        _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        _cinemachineCamera.Target.TrackingTarget = _camTarget;
    }

    private void LateUpdate()
    {
        Vector2 targetCamPos = CalculateTargetCamPos();
        Vector2 lerpPos = ((Vector2)_camTarget.position).Lerp(targetCamPos, _lerpSpeed * Time.deltaTime);
        _camTarget.position = (Vector3)lerpPos + Vector3.forward * _camTarget.position.z;
    }

    Vector2 CalculateTargetCamPos()
    {
        Vector2 targetPos = Vector2.zero;
        int count = 0;
        _playerManager.PlayerList.ForEach(player =>
        {
            targetPos += (Vector2)player.Actor.transform.position;
            count++;
        });
        if (count > 0)
        {
            targetPos /= count;
        }

        targetPos *= _centerOffsetMult;

        return targetPos;
    }
}
