using UnityEngine;

public class NewSkillUpgrade : Upgrade
{
    Skill _skillPrefab;
    Actor _targetActor;

    public NewSkillUpgrade(Skill skillPrefab, Actor targetActor)
    {
        this.SourceSkill = skillPrefab;
        this._skillPrefab = skillPrefab;
        this._targetActor = targetActor;
        _name = skillPrefab.gameObject.name;
        _icon = skillPrefab.Icon;
        _description = skillPrefab.Description;
    }

    public override string GetDescription()
    {
        var description = base.GetDescription();

        var se_statMod = _skillPrefab.GetComponent<SE_StatMod>();
        if(se_statMod != null)
        {
            foreach(var mod in se_statMod.Mods)
            {
                description = mod.GetUpgradePreviewString(_skillPrefab, _targetActor, description);
            }
        }

        return description;
    }

    public override Color GetColor()
    {
        if (_skillPrefab.SkillColor != Color.clear) return _skillPrefab.SkillColor;
        return GamePaletteManager.I.Palette.GetSkillColor(_skillPrefab);
    }

    public override void ApplyUpgrade()
    {
        var newSkill = _targetActor.SkillSystem.AddSkill(_skillPrefab);
        newSkill.Level++;
        Debug.Log("!!! Applying New Skill UPGRAde");
        _targetActor.Tags.AddTags(newSkill.Tags);
        _targetActor.OnUpgrade?.Invoke();
    }

    public override string GetSlot()
    {
        return _skillPrefab.Slot.ToString();
    }
}
