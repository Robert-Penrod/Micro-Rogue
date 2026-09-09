using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] bool _isLockedOn = false;
    [SerializeField] float _zoomLerpSpeed = 12f;
    [SerializeField] float _offsetLerpSpeed = 12f;

    float _targetZoom;
    Vector2 _targetPos;

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
        _targetPos = _camera.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (_isLockedOn)
        {
            _camera.Lens.OrthographicSize = _camera.Lens.OrthographicSize.Lerp(CalculateZoom() * _initZoom, _zoomLerpSpeed * Time.deltaTime);
            _camera.transform.localPosition = (Vector3)((Vector2)_camera.transform.localPosition.Lerp(CalculateOffset(), _offsetLerpSpeed * Time.deltaTime)) + Vector3.forward * _camera.transform.localPosition.z;
        }
        else
        {
            // Params
            float lerpSpeed = 6f;
            float zoomSpeed = 50f;
            float moveSpeed = 25f;

            // Get Inputs
            float zoomInput = 0f;
            zoomInput = zoomSpeed * -Input.mouseScrollDelta.y;
            Vector2 moveInput = Vector2.zero;
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();
            moveInput *= moveSpeed;

            // Apply transformations
            _targetZoom += zoomInput * Time.deltaTime;
            _targetZoom = _targetZoom.Clamp(8f, 16f);
            _camera.Lens.OrthographicSize = _camera.Lens.OrthographicSize.Lerp(_targetZoom, lerpSpeed * Time.deltaTime);
            //
            _targetPos += moveInput * Time.deltaTime;
            _targetPos.x = _targetPos.x.Clamp(-10f, 10f);
            _targetPos.y = _targetPos.y.Clamp(-10f, 10f);
            _camera.transform.localPosition = _camera.transform.localPosition.Lerp((Vector3)_targetPos + Vector3.forward * _camera.transform.localPosition.z, lerpSpeed * Time.deltaTime);
        }
    }
}
