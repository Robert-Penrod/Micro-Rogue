using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2Map
{
    public int Count
    {
        get
        {
            return _count;
        }
        set
        {
            _count = value;
            if (_count < 4)
            {
                _count = 4;
            }
            _deltaAngle = 360f / value;
            Clear();
        }
    }
    int _count;
    float _deltaAngle;

    public List<Vector2> Map = new List<Vector2>();

    public Vector2Map(int count, List<Vector2> vectorMapData = null)
    {
        Count = count;
        if(vectorMapData != null)
        {
            Map = new List<Vector2>(vectorMapData);
        }
    }

    public void Clear()
    {
        Map.Clear();
        for(int i = 0; i < Count; i++)
        {
            Map.Add(Vector2.zero);
        }
    }

    public void ForEach(Action<Vector2> callback)
    {
        for(int i = 0; i < Count; i++)
        {
            callback?.Invoke(GetDir(i));
        }
    }

    public Vector2 GetDir(int index)
    {
        return Quaternion.Euler(0f, 0f, index * _deltaAngle) * Vector2.up;
    }

    public void DebugDraw(Vector2 pos)
    {
        Map.ForEach(x =>
        {
            Debug.DrawLine(pos, pos + x);
        });
    }

    public void GizmoDraw(Vector2 pos)
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.125f);
        Map.ForEach(x =>
        {
            Gizmos.DrawLine(pos, pos + x);
        });
    }

    public Vector2 ToVector()
    {
        return Map.AverageVectors();
    }

    public Vector2Map Clone()
    {
        return new Vector2Map(Count, Map);
    }
}
