using UnityEngine;

public abstract class SkillEffect : MonoBehaviour
{
    protected Skill _skill;
    protected SkillTrigger _skillTrigger;
    public virtual void TriggerEffect()
    {

    }

    private void Awake()
    {
        _skill = GetComponent<Skill>();
        _skillTrigger = GetComponent<SkillTrigger>();

        if (this is SE_StatMod && (this as SE_StatMod).IsConstant) return;

        if (_skillTrigger != null)
        {
            _skillTrigger.OnTrigger += () =>
            {
                TriggerEffect();
            };
        }
    }
}
