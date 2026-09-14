using UnityEngine;

[CreateAssetMenu(menuName = "UnlockNode/StringUnlock")]
public class StringUnlock : ScriptableObject
{
    public string UnlockedString => this.name;
    public Sprite Sprite;
    public Color Color;
    public string Description;
}
