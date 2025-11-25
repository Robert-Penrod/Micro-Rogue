using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumTrackerEquipmentAnimator : MonoBehaviour
{
    public float Magnitude = 0.5f;
    MomentumTracker _momentumTracker;
    Vector3 _targetPos;
    Vector3 _initPos;

    private void Start()
    {
        _initPos = transform.localPosition;
        _initPos.z = 0;
        _momentumTracker = GetComponentInParent<MomentumTracker>();
        UpdateTargetPos();
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos, 12f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        UpdateTargetPos();
    }

    void UpdateTargetPos()
    {
        if(_momentumTracker == null)
        {
            _momentumTracker = GetComponentInParent<MomentumTracker>();
            if (_momentumTracker == null) return;
        }
        _targetPos = _initPos + Magnitude * (Vector3)_momentumTracker.MomentumVector + Vector3.forward * transform.localPosition.z;
    }
}
