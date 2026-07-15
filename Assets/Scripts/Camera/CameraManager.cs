using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] float _zoomLerpSpeed = 12f;
    [SerializeField] float _offsetLerpSpeed = 12f;

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

    Dictionary<object, Vector2> _offestDict = new();
    public Vector2 CalculateOffset()
    {
        Vector2 offest = Vector2.zero;
        foreach (var item in _offestDict)
        {
            offest += item.Value;
        }
        return offest;
    }
    public void Offset(Vector2 offest, object source)
    {
        if (_offestDict.ContainsKey(source))
        {
            _offestDict[source] = offest;
        }
        else
        {
            _offestDict.Add(source, offest);
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
        _camera.transform.localPosition = (Vector3)((Vector2)_camera.transform.localPosition.Lerp(CalculateOffset(), _offsetLerpSpeed * Time.deltaTime)) + Vector3.forward * _camera.transform.localPosition.z;
    }
}
