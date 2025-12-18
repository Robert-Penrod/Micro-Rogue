using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] float _zoomLerpSpeed = 12f;
    public float ZoomKnob = 1f;

    CinemachineCamera _camera;
    float _initZoom;

    protected override void Awake()
    {
        base.Awake();
        _camera = GetComponentInChildren<CinemachineCamera>();
        _initZoom = _camera.Lens.OrthographicSize;
    }

    private void LateUpdate()
    {
        _camera.Lens.OrthographicSize = _camera.Lens.OrthographicSize.Lerp(ZoomKnob * _initZoom, _zoomLerpSpeed * Time.deltaTime);
    }
}
