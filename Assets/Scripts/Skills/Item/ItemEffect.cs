using UnityEngine;

[RequireComponent(typeof(Skill))]
public class ItemEffect : MonoBehaviour
{
    protected Skill _skill;

    protected virtual void Awake()
    {
        _skill = GetComponent<Skill>();
    }
}
