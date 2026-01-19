using UnityEngine;

public abstract class SkillEffect : MonoBehaviour
{
    protected Skill _skill;
    protected SkillTrigger _skillTrigger;
    public virtual void Effect()
    {

    }

    private void Awake()
    {
        _skill = GetComponent<Skill>();
        _skillTrigger = GetComponent<SkillTrigger>();
        if (_skillTrigger != null)
        {
            _skillTrigger.OnTrigger += () =>
            {
                Effect();
            };
        }
    }
}
