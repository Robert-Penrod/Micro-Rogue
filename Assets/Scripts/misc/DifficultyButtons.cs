using System.Collections.Generic;
using UnityEngine;

public class DifficultyButtons : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> _selectSpritRends;

    private void Start()
    {
        SetSelection(1);
    }

    public void SetSelection(int index)
    {
        for(int i = 0; i < _selectSpritRends.Count; i++)
        {
            _selectSpritRends[i].color = i == index ? Color.green.SetSaturation(0.7f) : Color.white;
        }
    }
}
