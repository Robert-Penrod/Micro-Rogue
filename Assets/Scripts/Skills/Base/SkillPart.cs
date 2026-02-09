using UnityEngine;

public class SkillPart : MonoBehaviour
{
    public  Skill SourceSkill { get; private set; }
    protected SkillInstance _skillInstance;

    public virtual void SetSourceSkillInstance(SkillInstance skillInstance)
    {
        this.SourceSkill = skillInstance.Skill;
        this._skillInstance = skillInstance; 
        this._skillInstance.Link(skillInstance.Skill);
    }
}
