using UnityEngine;

public class NewSkillUpgrade : Upgrade
{
    Skill _skillPrefab;
    Actor _targetActor;

    public NewSkillUpgrade(Skill skillPrefab, Actor targetActor)
    {
        this._skillPrefab = skillPrefab;
        this._targetActor = targetActor;
        _name = skillPrefab.gameObject.name;
        _icon = skillPrefab.Icon;
        _description = skillPrefab.Description;
    }

    public override Color GetColor() => GamePaletteManager.I.Palette.GetSkillColor(_skillPrefab);

    public override void ApplyUpgrade()
    {
        _targetActor.SkillSystem.AddSkill(_skillPrefab);
    }
}
