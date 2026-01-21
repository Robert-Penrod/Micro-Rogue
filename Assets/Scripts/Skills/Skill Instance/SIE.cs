using UnityEngine;

public class SIE : MonoBehaviour
{
    protected SkillInstance _skillInstance;

    protected virtual void Awake()
    {
        _skillInstance = GetComponentInParent<SkillInstance>();
    }
}
