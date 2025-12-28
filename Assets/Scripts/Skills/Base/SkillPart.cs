using UnityEngine;

public class SkillPart : MonoBehaviour
{
    protected Skill _sourceSkill;

    public virtual void SetSourceSkill(Skill sourceSkill)
    {
        this._sourceSkill = sourceSkill;
    }
}
