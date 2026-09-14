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

            foreach (var upgrade in se_statMod.SkillUpgrade)
            {
                if (description != string.Empty) description += "\n";
                upgrade.SourceSkill = SourceSkill;
                description += upgrade.GetDescription();
                if(upgrade != se_statMod.SkillUpgrade.Last())
                {
                    description += "\n";
                }
            }
        }

        return description;
    }

    public override Color GetColor()
    {
        return _skillPrefab.GetColor();
    }

    public void ApplyUpgrade(Actor actor)
    {
        _targetActor = actor;
        ApplyUpgrade();
    }

    public override void ApplyUpgrade()
    {
        var newSkill = _targetActor.SkillSystem.AddSkill(_skillPrefab);
        newSkill.Level++;
        //Debug.Log("!!! Applying New Skill UPGRAde");
        _targetActor.Tags.AddTags(newSkill.Tags);
        _targetActor.OnUpgrade?.Invoke();

        // Skill Refresh
        _targetActor.SkillSystem.SkillList.ForEach(skill =>
        {
            skill.ReInitializeFromHistory();
        });
    }

    public override string GetSlot()
    {
        return _skillPrefab.Slot.ToString();
    }
}
