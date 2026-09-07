using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Skill : MonoBehaviour
{
    #region Vars
    public int GemCost = 0;
    public string Name => gameObject == null? string.Empty : Regex.Replace(gameObject.name, @"\s*\(.*$", "", RegexOptions.IgnoreCase);
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
    public enum SlotEnum { Main = 0, Offhand = 1, Passive = 2, Item = 3 }
    [VerticalGroup("Info/SkillGroup/Left")]
    public SlotEnum Slot;
    [VerticalGroup("Info/SkillGroup/Right")]
    public Color SkillColor = Color.clear;
    public Color GetColor()
    {
        if (SkillColor != Color.clear) return SkillColor;
        return GamePaletteManager.I.Palette.GetSkillColor(this);
    }

    [BoxGroup("Stats")]
    public SkillStats Stats;

    public TagCollection Tags;


    public SkillPrerequisites Prerequisites;
    [System.Serializable]
    public class SkillPrerequisites
    {
        public int OffhandCount;
        public TagCollection TagsRequired;
    }
    public bool ArePrerequisitesMet(Actor actor)
    {
        if(Prerequisites.OffhandCount > 0)
        {
            if(actor.SkillSystem.SkillList.FindAll(skill => skill.Slot == SlotEnum.Offhand).Count < Prerequisites.OffhandCount)
            {
                return false;
            }
        }

        var prereqTagList = Prerequisites.TagsRequired.GetTagList();
        if(prereqTagList.Count > 0)
        {
            foreach(var prereqTag in prereqTagList)
            {
                if (!actor.Tags.HasTag(prereqTag))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public Action OnCooldown;

    int _dir = 1;
    public int GetDirection(bool doToggle = false)
    {
        if(doToggle) _dir *= -1;
        return _dir;
    }

    [SerializeField] float _telegraphMult = 1f;
    public float TelegraphTime => /*(Actor?.Faction == Actor.FactionType.Enemy? 1.25f : 1f) * */ _telegraphMult * (Constants.SkillStats.BaseTelegraphTime + Constants.SkillStats.BaseTelegraphTime * (0.5f / Stats.Rate.Value) * (0.5f * Stats.Size.Value));
    [SerializeField] float _spawnDelayMult = 1f;
    public float SpawnDelayTime => _spawnDelayMult * TelegraphTime;// (Constants.SkillStats.SpawnDelay + TelegraphTime);

    [BoxGroup("Upgrades")]
    public List<UpgradeMod> NewUpgradeListTest = new();
    public List<SkillUpgrade> UpgradeList = new();
    public List<SkillUpgrade> UpgradeHistory = new();

    // Data
    [HideInInspector] public Actor Actor;

    // State
    public bool IsActive => SkillInstances.FindAll(skillInstance => skillInstance != null && (BlockCooldownDurringStart && skillInstance.State == SkillInstance.SkillInstanceState.Start) || (BlockCooldownDurringLifetime && skillInstance.State != SkillInstance.SkillInstanceState.End)).Count > 0;  // SkillInstances.Count > 0;
    [HideInInspector] public List<SkillInstance> SkillInstances = new();
    public bool NeedsTargetForCooldown = true;
    public float CooldownPercent { get; private set; }


    [BoxGroup("AI")]
    public float AttackChase = 1f;
    [BoxGroup("AI")]
    public float PassiveDistMult = 1f;
    [BoxGroup("AI")]
    public bool BlockCooldownDurringLifetime = false;
    [BoxGroup("AI")]
    public bool BlockCooldownDurringStart = true;
    SimpleNPCBrain _npcBrain;
    [BoxGroup("AI")]
    public bool IsUsableByNPCs = true;

    [SerializeField] float _dps;

    //Events
    public Action<float, Actor, Skill> OnHit;
    public Action<Actor> OnKill;

    [System.Serializable]
    public class MetaInfo
    {
        public int Kills;
        public float DamageDone;
    }
    public MetaInfo MetaSkillInfo;
    #endregion

    #region Init
    private void Start()
    {
        Stats.Init();
    }

    private void OnValidate()
    {
        _dps = (Stats.RandomDamage.Value * 0.5f + Stats.Damage.Value) * Stats.Rate.Value;

        // Init Upgrades
        UpgradeList.ForEach(upgrade =>
        {
            upgrade.SourceSkill = this;
        });
    }

    public void ReInitializeFromHistory()
    {
        float initHealthPercent = Actor.Stats.HealthPercent;
        //Debug.Log("REINITIALIZE SKILL");

        RemoveModsFromUpgradeHistory();
        

        UpgradeHistory.ForEach(Upgrade =>
        {
            Upgrade.ApplyMods();
        });

        Actor.Stats.SetHealthPercent(initHealthPercent);
    }

    public void RemoveModsFromUpgradeHistory()
    {
        UpgradeHistory.ForEach(upgrade =>
        {
            upgrade.RemoveMods();
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

        // Events
        OnHit = null;
        OnHit += (damage, hitActor, skill) =>
        {
            Actor.OnHit?.Invoke(damage, hitActor, skill);
            MetaSkillInfo.DamageDone += damage;
        };
        OnKill = null;
        OnKill += (slainActor) =>
        {
            Actor.OnKill?.Invoke(slainActor);
            MetaSkillInfo.Kills++;
        };

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
        float minMult = 0f;
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
            dodgeMult = Actor.MoveController.IsDodging ? -1f : 1f;
        }

        // No Targets
        if (NeedsTargetForCooldown)
        {
            if (Actor.Senses.EnemyActors.Count <= 0 || ((_npcBrain?.NoticeMag ?? 1f) < 1f))
            {
                if (CooldownPercent > 0.9f && dodgeMult > 0f)
                {
                    cooldownMult *= 0f;
                    //CooldownPercent -= 0.1f * Stats.Rate.Value * Time.fixedDeltaTime;
                }
            }
        }

        // Holding still boosts cooldown
        if(Actor.MoveController.MoveDir.magnitude <= 0.1f || Actor.Body.linearVelocity.magnitude < 0.1f)
        {
            //cooldownMult *= 1.125f;
        }

        // Paralysis Status
        if(Actor.IsParalyzed)
        {
            cooldownMult = 0f;
        }

        // Cooldown
        if (CooldownPercent < 1f)
        {
            float mult = cooldownMult * dodgeMult;
            //if (this.Slot == SlotEnum.Passive) mult = 1f;

            //mult *= Constants.SpeedMult;

            mult *= Actor.FrostSlowMult;

            CooldownPercent += mult * Stats.Rate.Value * Time.fixedDeltaTime;
            CooldownPercent = CooldownPercent.Clamp01();

            // On Cooldown
            if(CooldownPercent >= 1f)
            {
                OnCooldown?.Invoke();
            }
        }

        if(Stats.Rate.BaseValue <= 0f)
        {
            CooldownPercent = 1f;
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

        if (NewUpgradeListTest.Count > 0)
        {
            // NEW SYSTEM
            for (int upgradeCount = 6; upgradeCount > 0; upgradeCount--)
            {
                SkillUpgrade skillUpgrade = new();
                skillUpgrade.SourceSkill = this;
                int statCount = Random.Range(1, Mathf.Min(3, NewUpgradeListTest.Count + 1));
                for (int i = 0; i < statCount; i++)
                {
                    var element = NewUpgradeListTest.GetRandomElement();
                    if (!skillUpgrade.ModList.Contains(element))
                    {
                        skillUpgrade.ModList.Add(element);
                    }
                    else
                    {
                        i--;
                    }
                }
                skillUpgrade.ModList.ForEach(mod =>
                {
                    mod.BalancePoints = 1f / skillUpgrade.ModList.Count;
                });
                returnList.Add(skillUpgrade);
            }
        }
        else
        {
            // OLD SYTEM
            UpgradeList.ForEach(upgrade =>
            {
                if (upgrade.IsValid(this))
                {
                    returnList.Add(upgrade, Constants.RarityToWeight(upgrade.Rarity));
                }
            });
        }

        return returnList;
    }
    #endregion
}
