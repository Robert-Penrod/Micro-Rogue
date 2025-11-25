using UnityEngine;

[RequireComponent(typeof(SkillInstance))]
public class SIE : MonoBehaviour
{
    protected SkillInstance _skillInstance;

    protected virtual void Awake()
    {
        _skillInstance = GetComponent<SkillInstance>();
    }
}
