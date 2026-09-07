using System;
using System.Collections.Generic;
using UnityEngine;

public class SE_StatMod : SkillEffect
{
    public bool IsConstant = true;
    public List<UpgradeMod> Mods = new();
    public List<SkillUpgrade> SkillUpgrade = new();

    float _effectTick = float.MaxValue;

    private void OnEnable()
    {
        SkillUpgrade.ForEach(skillUpgrade =>
        {
            skillUpgrade.SourceSkill = _skill;
        });
        TriggerEffect();
    }

    private void Start()
    {
        // Instantiate skill upgrades
        SkillUpgrade.ForEach(skillUpgrade =>
        {
            skillUpgrade.SourceSkill = _skill;
        });

        // Reapply!
        _skill.Actor.OnUpgrade += () =>
        {

        };
    }

    private void Update()
    {
        if (IsConstant) return;

        if(_effectTick < _skill.Stats.Duration.Value)
        {
            _effectTick += Time.deltaTime;
            if(_effectTick >= _skill.Stats.Duration.Value)
            {
                // Effect wore off
            }
        }
    }

    void RemoveAllModifiersFromThisSource()
    {
        _skill.Actor.SkillSystem.SkillList.ForEach(skill =>
        {
            /*
            foreach(SkillStats.SkillStatTypes statEnum in Enum.GetValues(SkillStats.SkillStatTypes))
            {

            }
            */
            //skill.Stats.GetSkillStat()
        });
    }

    private void OnDestroy()
    {
        _skill.RemoveModsFromUpgradeHistory();
    }

    public override void TriggerEffect()
    {
        var actor = _skill?.Actor;
        float initHealthPercent = actor?.Stats.HealthPercent ?? 1f;

        SkillUpgrade.ForEach(skillUpgrade =>
        {
            if(_skill.Slot != Skill.SlotEnum.Item) _skill.Level--;
            skillUpgrade.SourceSkill = _skill;
            skillUpgrade.ApplyUpgrade();
        });

        UpgradeMod.ApplyModList(Mods, _skill);

        if (!IsConstant)
        {
            SpawnIcon();
        }

        if (actor != null)
        {
            actor.Stats.SetHealthPercent(initHealthPercent);
        }
        /*
        foreach (var upgradeMod in Mods)
        {
            if (upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.ActorStat)
            {
                var actorStats = _skill.Actor.Stats;
                var stat = actorStats.GetStat(upgradeMod.ActorStatName);
                string effectTag = _skill.Name + " - " + upgradeMod.ActorStatName.ToString();
                stat.RemoveAllModifiersWithTag(effectTag);
                var mod = upgradeMod.GetModifier();
                mod.Tags.Add(effectTag);
                mod.Source = this;
                mod.IsStackable = true;
                stat.AddModifier(mod);
            }
            else if (upgradeMod.TargetType == UpgradeMod.UpgradeTargetType.GlobalSkillStat)
            {
                foreach (var skill in _skill.Actor.SkillSystem.SkillList)
                {
                    if (!upgradeMod.IsSkillValid(skill)) continue;

                    var stat = skill.Stats.GetSkillStat(upgradeMod.SkillStatName);
                    var mod = upgradeMod.GetModifier();
                    string effectTag = _skill.name;
                    mod.Tags.Add(effectTag);
                    mod.Source = this;
                    mod.IsStackable = true;
                    stat.AddModifier(mod);

                    Debug.Log("Doing global stat mod ");
                    Debug.Log(upgradeMod.SkillStatName.ToString());
                    Debug.Log(mod.Value);
                }
            }
        }
        */
    }

    void SpawnIcon()
    {
        Sprite icon = _skill.Icon;
        GameObject iconObject = new GameObject();
        SpriteRenderer spriteRend = iconObject.AddComponent<SpriteRenderer>();
        spriteRend.sprite = icon;
        //spriteRend.color = GetComponent<ActorSkillColorer>().TargetColor.GetCopyWithAlpha(0.5f);

        iconObject.transform.position = transform.position;
        iconObject.transform.localScale = Vector3.one * 0.5f;

        Rigidbody2D parentBody = GetComponentInParent<Rigidbody2D>();
        Rigidbody2D rb = iconObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 3f;

        rb.linearVelocity = parentBody.linearVelocity;
        rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

        /*
        SpriteFadeDestroyer fadeDestroyer = iconObject.AddComponent<SpriteFadeDestroyer>();
        fadeDestroyer.DestroyTime = 0.5f;
        fadeDestroyer.FadeTime = 0.5f;
        */
    }
}
