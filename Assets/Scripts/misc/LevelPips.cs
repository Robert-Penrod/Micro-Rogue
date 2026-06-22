using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPips : MonoBehaviour
{
    List<Image> _imageList = new();

    private void Awake()
    {
        _imageList.AddRange(GetComponentsInChildren<Image>());
        _imageList.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
    }

    private void Start()
    {
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            SetLevel(DungeonManager.I.Data.Coordinate.y);
        };
        SetLevel(DungeonManager.I.Data.Coordinate.y);
    }

    void SetLevel(int level)
    {
        int pipLevel = 1 + ((level-1) % 10);
        for(int i = 0; i < _imageList.Count; i++)
        {
            _imageList[i].color = i < pipLevel ? Color.white.Alpha(0.675f) : Color.grey.Alpha(0.375f);
        }
    }
}
