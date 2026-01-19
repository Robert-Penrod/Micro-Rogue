using UnityEngine;

public class SkillPart : MonoBehaviour
{
    protected Skill _sourceSkill;
    protected SkillInstance _skillInstance;

    public virtual void SetSourceSkillInstance(SkillInstance skillInstance)
    {
        this._sourceSkill = skillInstance.Skill;
        this._skillInstance = skillInstance;
    }
}
