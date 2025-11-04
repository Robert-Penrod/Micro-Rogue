using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullColor : MonoBehaviour
{
    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            _color = value;
            UpdateColor();
        }
    }
    Color _color;
    public List<SpriteRenderer> PrimarySprites;
    public List<SpriteRenderer> SecondarySprites;

    public List<KeyCode> ActivationKeys;

    private void Awake()
    {
        PrimarySprites.Add(GetComponent<SpriteRenderer>());
        PrimarySprites.Add(transform.GetChild(1).GetComponent<SpriteRenderer>());
        SecondarySprites.Add(transform.GetChild(0).GetComponent<SpriteRenderer>());

        RandomizeColor();
    }

    private void Update()
    {
        if(ActivationKeys.GetKeyDown())
        {
            RandomizeColor();
        }
    }

    void RandomizeColor()
    {
        Color = Color.white;// Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 0.5f), 1f);
    }

    void UpdateColor()
    {
        PrimarySprites.ForEach(x => x.color = Color);
        SecondarySprites.ForEach(y => y.color = Color.SetValue(0.3f));
    }
}
