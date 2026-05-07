using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] float _zoomLerpSpeed = 12f;

    CinemachineCamera _camera;
    float _initZoom;

    Dictionary<object, float> _zoomDict = new();
    float CalculateZoom()
    {
        float zoom = 1f;
        foreach(var item in _zoomDict)
        {
            zoom *= item.Value;
        }
        return zoom;
    }
    public void Zoom(float mult, object source)
    {
        if(_zoomDict.ContainsKey(source))
        {
            _zoomDict[source] = mult;
        }
        else
        {
            _zoomDict.Add(source, mult);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _camera = GetComponentInChildren<CinemachineCamera>();
        _initZoom = _camera.Lens.OrthographicSize;
    }

    private void LateUpdate()
    {
        _camera.Lens.OrthographicSize = _camera.Lens.OrthographicSize.Lerp(CalculateZoom() * _initZoom, _zoomLerpSpeed * Time.deltaTime);
    }
}
