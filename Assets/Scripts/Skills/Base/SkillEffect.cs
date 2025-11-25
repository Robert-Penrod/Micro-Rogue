using UnityEngine;

public abstract class SkillEffect : MonoBehaviour
{
    protected Skill _skill;
    protected SkillTrigger _skillTrigger;
    public abstract void Effect();

    private void Awake()
    {
        _skill = GetComponent<Skill>();
        _skillTrigger = GetComponent<SkillTrigger>();
        _skillTrigger.OnTrigger += () =>
        {
            Effect();
        };
    }
}
