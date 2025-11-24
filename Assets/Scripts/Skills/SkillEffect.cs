using UnityEngine;

public abstract class SkillEffect : MonoBehaviour
{
    protected SkillTrigger _skillTrigger;
    public abstract void Effect();

    private void Awake()
    {
        _skillTrigger = GetComponent<SkillTrigger>();
        _skillTrigger.OnTrigger += () =>
        {
            Effect();
        };
    }
}
