using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Skill : MonoBehaviour
{
    [Header("Info")]
    [TextArea] public string Description;
    public Sprite Icon;
    public Color SkillColor = Color.clear;
    [SerializeField] float _dps;

    [Header("Stats")]
    public SkillStats Stats;

    // Data
    [HideInInspector] public Actor Actor;

    // State
    [Header("State")]
    public bool IsActive => SkillInstances.Count > 0;
    public List<SkillInstance> SkillInstances = new();
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
        float skillSpeed = 1f;
        float cooldown = Constants.SkillStats.Cooldown.Default;
        float mainSkillMult = (_tempSkillList.FindAll(skill => (skill.Stats.Slot == SkillStats.SlotEnum.Main) && skill.IsActive).Count > 0)? 0f : 1f;
        float offhandSkillMult = (_tempSkillList.FindAll(skill => (skill.Stats.Slot == SkillStats.SlotEnum.Offhand) && skill.IsActive).Count > 0) ? 0.5f : 1f;
        if (this.Stats.Slot == SkillStats.SlotEnum.Offhand)
        {
            //mainSkillMult = 1f;
        }
        else if(this.Stats.Slot == SkillStats.SlotEnum.Passive)
        {
            mainSkillMult = 1f;
            offhandSkillMult = 1f;
        }

        float dodgeMult = 1f;
        if (Actor.MoveController != null)
        {
            dodgeMult = Actor.MoveController.IsDodging ? 0f : 1f;
        }


        // Cooldown
        if(CooldownPercent < 1f)
        {
            CooldownPercent += dodgeMult * offhandSkillMult * mainSkillMult * (skillSpeed / cooldown) * Time.fixedDeltaTime;
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
