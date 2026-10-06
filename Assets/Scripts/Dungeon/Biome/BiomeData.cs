using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BiomeData")]
public class BiomeData : ScriptableObject
{
    public DungeonManager.BiomeEnum BiomeType;
    public Sprite Icon;
    public Color Color;
    public List<AudioClip> _ambience = new();
    public List<Sprite> _textures = new();
}
