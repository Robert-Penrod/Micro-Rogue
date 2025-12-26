using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Skill : MonoBehaviour
{
    public string Name => Regex.Replace(this.gameObject.name, @"\s*\(.*$", "");
    [BoxGroup("Info")]
    [HorizontalGroup("Info/SkillGroup", 75), VerticalGroup("Info/SkillGroup/Left")]
    [PreviewField(75, ObjectFieldAlignment.Left, FilterMode = FilterMode.Point)]
    [HideLabel]
    public Sprite Icon;
    [VerticalGroup("Info/SkillGroup/Right", 0.2f)]
    public Constants.Rarity Rarity;
    [VerticalGroup("Info/SkillGroup/Right", 0.2f)]
    [TextArea] public string Description;
    public enum SlotEnum { Main = 0, Offhand = 1, Passive = 2 }
    [VerticalGroup("Info/SkillGroup/Left")]
    public SlotEnum Slot;
    [VerticalGroup("Info/SkillGroup/Right")]
    public Color SkillColor = Color.clear;

    [BoxGroup("Stats")]
    public SkillStats Stats;

    [SerializeField] float _dps;

    [BoxGroup("Upgrades")]
    public List<SkillUpgrade> UpgradeList = new();

    // Data
    [HideInInspector] public Actor Actor;

    // State
    public bool IsActive => SkillInstances.Count > 0;
    [HideInInspector] public List<SkillInstance> SkillInstances = new();
    public float CooldownPercent { get; private set; }

    bool _wasNoTargets = false;

    private void OnValidate()
    {
        _dps = Stats.Damage.Value * Stats.Rate.Value;
    }

    private void OnEnable()
    {
        Actor = GetComponentInParent<Actor>();
        ShuffleCooldown();
        _tempSkillList.Clear(); _tempSkillList.AddRange(new List<Skill>(transform.parent.GetComponentsInChildren<Skill>()));
    }
    List<Skill> _tempSkillList = new();

    private void FixedUpdate()
    {
        // Temp Stats
        float activeSkillMult = 1f;
        float mainSkillMult = (_tempSkillList.FindAll(skill => (skill.Slot == Skill.SlotEnum.Main) && skill.IsActive).Count > 0)? 0f : 1f;
        float offhandSkillMult = (_tempSkillList.FindAll(skill => (skill.Slot == Skill.SlotEnum.Offhand) && skill.IsActive).Count > 0) ? 0f : 1f;
        switch (this.Slot)
        {
            case SlotEnum.Main:
                activeSkillMult *= mainSkillMult;
                activeSkillMult *= offhandSkillMult.RemapPercent(0.5f, 1f);
                break;
            case SlotEnum.Offhand:
                activeSkillMult *= offhandSkillMult;
                activeSkillMult *= mainSkillMult.RemapPercent(0.5f, 1f);
                break;
        }

        float dodgeMult = 1f;
        if (Actor.MoveController != null)
        {
            dodgeMult = Actor.MoveController.IsDodging ? 0f : 1f;
        }


        // Cooldown
        if(CooldownPercent < 1f)
        {
            CooldownPercent += activeSkillMult * dodgeMult * Stats.Rate.Value * Time.fixedDeltaTime;
            CooldownPercent = CooldownPercent.ClampMax(1f);
        }

        // No Targets
        if(Actor.Senses.EnemyActors.Count <= 0)
        {
            CooldownPercent = CooldownPercent.ClampMax(0.75f);
            _wasNoTargets = true;
        }
        else if(_wasNoTargets)
        {
            _wasNoTargets = false;
            CooldownPercent *= Random.Range(0.5f, 1f);
        }
    }

    public void ResetCooldown()
    {
        CooldownPercent = 0f;
        CooldownPercent += 0.1f * Random.Range(-1f, 1f);
    }

    public void ShuffleCooldown()
    {
        //UnityEngine.Random.InitState(DateTime.Now.Ticks.GetHashCode());
        CooldownPercent = Random.Range(0f, 0.5f);
    }
}
