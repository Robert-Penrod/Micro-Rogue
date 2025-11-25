using HyperQuest.EasyPooling;
using UnityEngine;

public class SkillInstance_CC : ColorController, IPoolable
{
    [SerializeField] SpriteRenderer _mainSprite;

    private void Awake()
    {
        UpdateColor();
    }

    public void Initialize()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        Color c = _mainSprite?.color ?? Color.grey;
        int colorCount = 1;



        c /= colorCount;

        SetColor(c);
    }
}
