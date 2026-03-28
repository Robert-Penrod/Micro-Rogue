using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Skill : MonoBehaviour
{
    #region Vars
    public string Name => Regex.Replace(gameObject.name, @"\s*\(.*$", "", RegexOptions.IgnoreCase);
    [BoxGroup("Info")]
    [HorizontalGroup("Info/SkillGroup", 75), VerticalGroup("Info/SkillGroup/Left")]
    [PreviewField(75, ObjectFieldAlignment.Left, FilterMode = FilterMode.Point)]
    [HideLabel]
    public Sprite Icon;
    [VerticalGroup("Info/SkillGroup/Right", 0.2f)]
    public int Level = 0;
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

    public TagCollection Tags;

    [SerializeField] float _dps;
    int _dir = 1;
    public int GetDirection(bool doToggle = false)
    {
        if(doToggle) _dir *= -1;
        return _dir;
    }

    public float TelegraphTime => /*(Actor?.Faction == Actor.FactionType.Enemy? 1.25f : 1f) * */ (Constants.SkillStats.BaseTelegraphTime + Constants.SkillStats.BaseTelegraphTime * (0.5f / Stats.Rate.Value) * (0.5f * Stats.Size.Value));

    [BoxGroup("Upgrades")]
    public List<SkillUpgrade> UpgradeList = new();
    public List<SkillUpgrade> UpgradeHistory = new();

    // Data
    [HideInInspector] public Actor Actor;

    // State
    public bool IsActive => SkillInstances.FindAll(skillInstance => (BlockCooldownDurringStart && skillInstance.State == SkillInstance.SkillInstanceState.Start) || (IsActiveDurringLifetime && skillInstance.State != SkillInstance.SkillInstanceState.End)).Count > 0;  // SkillInstances.Count > 0;
    [HideInInspector] public List<SkillInstance> SkillInstances = new();
    public bool NeedsTargetForCooldown = true;
    public float CooldownPercent { get; private set; }


    [BoxGroup("AI")]
    public float AttackChase = 1f;
    [BoxGroup("AI")]
    public float PassiveDistMult = 1f;
    [BoxGroup("AI")]
    public bool IsActiveDurringLifetime = false;
    [BoxGroup("AI")]
    public bool BlockCooldownDurringStart = true;
    SimpleNPCBrain _npcBrain;
    #endregion

    #region Init
    private void OnValidate()
    {
        _dps = (Stats.RandomDamage.Value * 0.5f + Stats.Damage.Value) * Stats.Rate.Value;

        // Init Upgrades
        UpgradeList.ForEach(upgrade =>
        {
            upgrade.SourceSkill = this;
        });
    }

    private void OnEnable()
    {
        // Init Upgrades
        UpgradeList.ForEach(upgrade =>
        {
            upgrade.SourceSkill = this;
        });

        // References
        Actor = GetComponentInParent<Actor>();
        _npcBrain = GetComponentInParent<SimpleNPCBrain>();

        // Cooldown
        ShuffleCooldown();
    }
    #endregion

    #region Update / Cooldown
    private void FixedUpdate()
    {
        float cooldownMult = 1f;

        // Temp Stats
        float activeSkillMult = 1f;
        var skillList = Actor.SkillSystem.SkillList;
        float minMult = 0.1f;
        float mainSkillMult = (skillList.FindAll(skill => (skill.Slot == Skill.SlotEnum.Main) && skill.IsActive).Count > 0)? minMult : 1f;
        float offhandSkillMult = (skillList.FindAll(skill => (skill.Slot == Skill.SlotEnum.Offhand) && skill.IsActive).Count > 0) ? minMult : 1f;
        switch (this.Slot)
        {
            case SlotEnum.Main:
                activeSkillMult *= mainSkillMult;
                activeSkillMult *= offhandSkillMult.RemapPercent(0.25f, 1f);
                break;
            case SlotEnum.Offhand:
                activeSkillMult *= offhandSkillMult;
                activeSkillMult *= mainSkillMult.RemapPercent(0.25f, 1f);
                break;
        }
        cooldownMult *= activeSkillMult;

        float dodgeMult = 1f;
        if (Actor.MoveController != null)
        {
            dodgeMult = Actor.MoveController.IsDodging ? 0f : 1f;
        }

        // No Targets
        if (NeedsTargetForCooldown)
        {
            if (Actor.Senses.EnemyActors.Count <= 0 || ((_npcBrain?.NoticeMag ?? 1f) < 1f))
            {
                if (CooldownPercent > 0.9f)
                {
                    cooldownMult *= 0f;
                    //CooldownPercent -= 0.1f * Stats.Rate.Value * Time.fixedDeltaTime;
                }
            }
        }

        // Holding still boosts cooldown
        if(Actor.MoveController.MoveDir.magnitude <= 0.1f || Actor.Body.linearVelocity.magnitude < 0.1f)
        {
            cooldownMult *= 1.125f;
        }

        // Cooldown
        if (CooldownPercent < 1f)
        {
            float mult = cooldownMult * dodgeMult;
            if (this.Slot == SlotEnum.Passive) mult = 1f;

            //mult *= Constants.SpeedMult;

            CooldownPercent += mult * Stats.Rate.Value * Time.fixedDeltaTime;
            CooldownPercent = CooldownPercent.ClampMax(1f);
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
    #endregion

    #region Upgrade
    public WeightedList<Upgrade> GetUpgradeListClone()
    {
        WeightedList<Upgrade> returnList = new();

        UpgradeList.ForEach(upgrade =>
        {
            if (upgrade.IsValid(this))
            {
                returnList.Add(upgrade, Constants.RarityToWeight(upgrade.Rarity));
            }
        });

        return returnList;
    }
    #endregion
}
