using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class RandomChildEffector : MonoBehaviour
{
    public bool TriggerOnAwake;
    public bool TriggerOnEnable;
    public bool RandomScale = false;
    public bool RandomFlip;
    public float RotationSnap = 0f;
    public Action OnActivate;

    public float RandomScaleMult;
    Vector3 _initScale;

    public bool ActivateChildren = true;
    [SerializeField] int _childCount = 1;

    private void OnEnable()
    {
        if(TriggerOnEnable)
        {
            Activate(_childCount);
        }
    }

    private void Awake()
    {
        _initScale = transform.localScale;
        if (TriggerOnAwake)
        {
            Activate(_childCount);
        }
    }

    public void Activate(int count)
    {
        _childCount = count;
        Activate();
    }

    public void Activate()
    {
        if (ActivateChildren) { HandleChildActivation(); }
        HandleTransformations();
        OnActivate?.Invoke();
    }

    void HandleTransformations()
    {
        if (RandomScale)
        {
            HandleScale();
        }
            HandleFlip();
        HandleRotation();
    }

    void HandleFlip()
    {
        // Random Flip
        if (RandomFlip)
        {
            Vector3 s = transform.localScale;
            float sx = s.x;
            float sy = s.y;
            sx *= Random.value > 0.5f ? 1 : -1;
            sy *= Random.value > 0.5f ? 1 : -1;
            transform.localScale = new Vector3(sx, sy, s.z);
        }
    }

    void HandleRotation()
    {
        if (RotationSnap > 0f)
        {
            float rotationSnap = RotationSnap;
            int rotationCount = (int)(360f / rotationSnap);
            int angleIndex = Random.Range(0, rotationCount);
            float angle = angleIndex * rotationSnap;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void HandleChildActivation()
    {
        // Deactivate all children
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }

        // Get child list
        List<Transform> deactivatedChildren = new List<Transform>();
        foreach (Transform t in transform)
        {
            deactivatedChildren.Add(t);
        }

        // /Choose _childCount random children to activate
        for (int i = 0; i < _childCount && deactivatedChildren.Count > 0; i++)
        {
            Transform chosenChild = deactivatedChildren.GetRandomElement();
            deactivatedChildren.Remove(chosenChild);
            chosenChild.gameObject.SetActive(true);
        }
    }

    void HandleScale()
    {
        transform.localScale = _initScale * Random.Range(1f - RandomScaleMult, 1f + RandomScaleMult);
    }
}
