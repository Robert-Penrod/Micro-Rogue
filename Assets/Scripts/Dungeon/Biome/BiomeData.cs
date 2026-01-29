using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BiomeData")]
public class BiomeData : ScriptableObject
{
    public Sprite Icon;
    public Color Color;
    public List<Sprite> _textures = new();
}
