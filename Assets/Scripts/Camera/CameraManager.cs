using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] bool _isLockedOn = false;
    [SerializeField] float _zoomLerpSpeed = 12f;
    [SerializeField] float _offsetLerpSpeed = 12f;
    [SerializeField] CamTarget _camTarget;

    float _minZoom = 8f;
    float _maxZoom = 16f;
    float _maxPos = 10f;

    Vector2 _targetPos;
    float _targetZoom;
    Vector2 _dragOrigin;

    CinemachineCamera _camera;
    float _initZoom;

    public void LockOnCamera()
    {
        _isLockedOn = true;
    }

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
        _targetZoom = _initZoom;
        _targetPos = transform.localPosition;
    }

    private void Update()
    {
        _camTarget.enabled = _isLockedOn;
        if (_isLockedOn)
        {
            _camera.Lens.OrthographicSize = _camera.Lens.OrthographicSize.Lerp(CalculateZoom() * _initZoom, _zoomLerpSpeed * Time.deltaTime);
            _camera.transform.localPosition = (Vector3)((Vector2)_camera.transform.localPosition.Lerp(CalculateOffset(), _offsetLerpSpeed * Time.deltaTime)) + Vector3.forward * _camera.transform.localPosition.z;
        }
        else
        {
            var lerpSpeed = 1f;
            var selectedObj = EventSystem.current.currentSelectedGameObject;
            if (selectedObj == null) return;

            Vector3 targetPos = -1.5f * Vector3.up + selectedObj.transform.position.SetZ(_camera.transform.localPosition.z);
            _camera.transform.localPosition = _camera.transform.localPosition.Lerp(targetPos, lerpSpeed * Time.deltaTime);
        }
    }

    Vector3 ClampPosVector(Vector3 v)
    {
        v.x = v.x.Clamp(-_maxPos, _maxPos);
        v.y = v.y.Clamp(-_maxPos, _maxPos);
        return v;
    }
}
