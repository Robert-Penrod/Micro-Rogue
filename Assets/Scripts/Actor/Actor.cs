using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] int _lvl;
    [SerializeField] ActorSkillSystem _skillSystem;

    [Header("Config")]
    public float RarityMult = 1f;
    public float Difficulty = 1f;
    public float UpgradeAffinity = 1f;
    public float NewSkillAffinity = 1f;
    public List<TagCollection.TagType> BlacklistedTags;
    public List<Skill> InnateSkillList = new();
    public FactionType Faction = FactionType.Enemy;
    public enum FactionType { None, Player, Enemy }
    public ActorStats Stats;
    public TagCollection Tags;
    [Header("Biomes")]
    public float WildsAffinity = 0f;
    public float UndergroundAffinity = 0f;
    public float DungeonAffinity = 0f;

    // Data
    [Header("State")]
    public bool IsAlive { get; private set; }
    public float _initScale;
    float _initialLinearDamping;

    // References
    public GameObject DeathDrop;
    public AudioClip DeathClip;
    public Rigidbody2D Body { get; private set; }
    public ActorMoveController MoveController { get; private set; }
    public ActorSenses Senses { get; private set; }
    public ActorSkillSystem SkillSystem { get; private set; }

    // Events
    public Action OnTakeDamage;
    public Action OnWasHit;
    public Action OnUpgrade;
    public Action OnDeath;
    public Action OnEvade;
    public Action OnArmor;
    public Action<Actor> OnKill;
    public Action<float, Skill> OnHit;

    public class MetaInfo
    {
        public int Kills;
        public int DamageDone;
    }

    [SerializeField] SpriteRenderer _spriteRend;
    public Sprite Sprite => _spriteRend.sprite;
    public Color Color => _spriteRend.color;

    [SerializeField] Sprite  _armorSprite;
    [SerializeField] AudioClip _armorSound;
    [SerializeField] Sprite _evadeSprite;
    [SerializeField] AudioClip _evadeSound;

    Color _initColor;

    public int GetLevel()
    {
        if (SkillSystem == null) return 0;

        int level = 0;
        SkillSystem.SkillList.ForEach(skill =>
        {
            level += skill.Level;
        });
        return level;
    }

    private void Awake()
    {
        DeathDrop.gameObject.SetActive(false);
        Body = GetComponent<Rigidbody2D>();
        _initialLinearDamping = Body.linearDamping;
        MoveController = GetComponent<ActorMoveController>();
        Senses = GetComponent<ActorSenses>();
        SkillSystem = GetComponentInChildren<ActorSkillSystem>();

        // Init
        IsAlive = true;
        Stats.HealthMax.BaseValue = Stats.Health;
        Stats.SetHealthPercent(1f);

        // Sprite Rend
        _initColor = _spriteRend.color;
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
        _spriteRend.material.SetVector("_TextureOffset", new Vector2(1000f * Random.Range(-1f, 1f), 1000f * Random.Range(-1f, 1f)));
        _spriteRend.material.SetFloat("_TextureSize", _spriteRend.material.GetFloat("_TextureSize") * Random.Range(0.9f, 1.1f));

        // Scale
        _initScale = transform.localScale.x;
        transform.localScale = Vector3.zero;

        // Health Change
        Stats.OnHealthChanged += (float newHp, float deltaHp) =>
        {
            if (newHp <= 0 && IsAlive) Die();
        };

        // Info Updates
        OnUpgrade += () =>
        {
            Debug.Log(this.gameObject.name + " OnUpgrade -> Updating Level");
            _lvl = GetLevel();
        };
        _skillSystem = SkillSystem;
    }

    void Die()
    {
        gameObject.SetCollidersEnabled2D(false);
        IsAlive = false;

        if (!IsPlayer())
        {
            this.DelayedInvoke(0.25f * Random.Range(0.9f, 1.1f), () =>
            {
                // Death Drop
                DeathDrop.transform.SetParent(null);
                DeathDrop.gameObject.SetActive(true);
                var pSystemList = new List<ParticleSystem>(DeathDrop.GetComponentsInChildren<ParticleSystem>());
                var p = DeathDrop.GetComponent<ParticleSystem>();
                pSystemList.Add(p);

                foreach(ParticleSystem pSystem in pSystemList)
                {
                    if (pSystem != null)
                    {
                        var main = pSystem.main;
                        main.startColor = _initColor.Alpha(main.startColor.color.a);
                        pSystem.Play();
                    }
                }
                
                AudioSpawner.PlayAudioWithRandPitch(DeathClip, 0.2f, 1f, 1f, transform.position);

                // Destroy
                Destroy(this.gameObject);
            });
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        OnDeath?.Invoke();

        Debug.Log($"{this.gameObject.name} died");
    }

    public bool IsInUI { get; private set; }
    Vector3 _preUIPos;
    private void Update()
    {
        ScaleUpdate();
    }

    public void SetInUI(bool isInUI)
    {
        if (this.IsInUI == isInUI) return;
        this.IsInUI = isInUI;
        this.gameObject.layer = IsInUI ? LayerMask.NameToLayer("UI") : LayerMask.NameToLayer("Actor");

        if (IsInUI)
        {
            _preUIPos = transform.position;
            transform.position = -100f * Vector3.forward;
        }
        else
        {
            transform.position = _preUIPos;
            transform.localScale = _initScale * Vector3.one;
        }

        if(IsPlayer())
        {
            MoveController.enabled = !IsInUI;
        }
    }

    private void FixedUpdate()
    {
        //HandleMoveFixedUpdate();
    }

    public int Heal(int heal, SkillInstance sourceSkillInstance, Actor sourceActor)
    {
        heal = (int)Mathf.Min(heal, Stats.HealthMax.Value - Stats.Health);
        if (heal <= 0) return 0;

        Stats.Health += heal;

        Color c = Color.green.SetSaturation(0.7f);
        SpawnPopup(heal, null, c, Vector2.zero, transform.position);

        return heal;
    }

    public int TakeDamage(int damage, SkillInstance sourceSkillInstance, Actor actor)
    {
        string blockType = string.Empty;
        // Whiff
        if(damage <= 0)
        {
            return 0;
        }

        // Dodge - GrazeFrames
        if (MoveController.IsDodging)
        {
            damage = 0;
            blockType = "dodge";
            /*
            damage /= 5;
            if(damage <= 0)
            {
                // Evasion
                blockType = "dodge";
            }
/**/
        }
        // Dodge - IFrames
        //if (MoveController.IsDodging) damage *= 0;

        Utils.RandomSeed();

        // Armor
        int armorRoll = (int)Random.Range(0f, Stats.Defense.Value + Stats.Defense.Value.Sign() * 0.99f);
        damage -= armorRoll;

        // Evasion
        int evasionRoll = (int)Random.Range(0f, Stats.Evasion.Value + Stats.Evasion.Value.Sign() * 0.99f);
        damage -= evasionRoll;


        if (damage <= 0)
        {
            if (blockType == string.Empty)
            {
                // Armor Block
                if (armorRoll > evasionRoll)
                {
                    blockType = "armor";
                    OnArmor?.Invoke();
                    PlayAudio(_armorSound);
                }
                // Evasion Block
                else
                {
                    blockType = "evade";
                    OnEvade?.Invoke();
                    PlayAudio(_evadeSound);
                }
            }
            damage = 0;
        }

        // Hit Event
        if(blockType != "dodge")
        {
            OnWasHit?.Invoke();
        }

        // Last Chance (Players: If killing hit would do more than x% health -> leave player at 1hp instead)
        if (IsPlayer())
        {
            if (damage >= Stats.Health && damage >= 0.25f * Stats.HealthMax.Value && Stats.Health > 1)
            {
                damage = Stats.Health - 1;
            }
        }

        // Do damage
        Stats.Health -= damage;
        OnTakeDamage?.Invoke();

        Sprite blockSprite = null;
        Color blockColor = Color.clear;
        if(blockType == "armor")
        {
            blockSprite = _armorSprite;
            blockColor = _spriteRend.color;
        }
        else if(blockType == "evade")
        {
            blockSprite = _evadeSprite;
            blockColor = _spriteRend.color;
        }

        // Popup
        if (sourceSkillInstance != null)
        {
            var sourceSkillInstanceBody = sourceSkillInstance.GetComponent<Rigidbody2D>();
            var normalColor = GamePaletteManager.I.Palette.GetActorSkillColor(sourceSkillInstance.Skill);
            Color c = blockColor != Color.clear? blockColor.Lerp(normalColor, 0.1f) : normalColor;
            Vector2 vel = sourceSkillInstanceBody?.linearVelocity ?? Vector2.zero;
            SpawnPopup(damage <= 0? null : -damage, blockSprite, c, 0.25f * vel, sourceSkillInstance.transform.position);
        }
        else if (actor != null)
        {
            if (actor.Faction != FactionType.None)
            {
                var palette = GamePaletteManager.I.Palette;
                var normalColor = (actor.Faction == FactionType.Player ? palette.PlayerColor : palette.EnemyColor);
                Color c = blockColor != Color.clear ? blockColor.Lerp(normalColor, 0.1f) : normalColor;
                Vector2 vel = 0.5f * actor.Body.linearVelocity + 0.5f * Body.linearVelocity;
                SpawnPopup(damage <= 0 ? null : -damage, blockSprite, c, vel, transform.position);
            }
        }

        // Return
        return damage;
    }

    void PlayAudio(AudioClip clip)
    {
        AudioSpawner.PlayAudioWithRandPitch(clip, 0.2f, 1f, 0.5f, transform.position);
    }

    void SpawnPopup(int? value, Sprite sprite, Color c, Vector2 vel, Vector2 pos)
    {
        // color
        string colorString = "#" + ColorUtility.ToHtmlStringRGB(c.Lerp(Color.white, 0.25f));// hitActor.Faction == Actor.FactionType.Player ? "#FF9900" : "#FFFFFF";

        // string
        string popupString = string.Empty;
        if (value != null)
        {
            string symbol = value > 0 ? "+" : string.Empty;
            popupString = "<color=" + colorString + ">" + symbol + value.ToString() + "</color>";
        }

        // popup
        Vector3 popupPos = Vector2.Lerp(pos, transform.position, IsAlive ? 0.5f : 1f);
        popupPos += 0.25f * (Vector3)Random.insideUnitCircle;
        SymbolPopup2DManager.I.CreatePopup(popupPos, popupString, sprite, c,  0.5f * vel, IsAlive ? transform : null);
    }

    public void Rest()
    {
        float restFactor = 0.4f;// 0.3f; // 0.4f; // 0.375f; // Prototype was 0.25f
        Heal((int)(Stats.HealthMax.Value * restFactor), null, this);
    }

    public void DeepRest()
    {
        Stats.Health = (int)(Stats.HealthMax.Value * 2f);
    }

    public bool IsPlayer()
    {
        return gameObject?.CompareTag("Player") ?? false;
    }

    public bool IsEnemyOf(Actor otherActor)
    {
        if (Faction == FactionType.None) return false;
        if (otherActor.Faction == FactionType.None) return false;
        if (otherActor.Faction == Faction) return false;

        return true;
    }

    public bool HasLineOfSightOf(Actor otherActor) => HasLineOfSightOf(otherActor.transform.position);
    public bool HasLineOfSightOf(Vector2 position)
    {
        Vector2 dir = position - (Vector2)transform.position;
        float dist = dir.magnitude;
        bool prevQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + dir.normalized * dist, LayerMask.GetMask("Default"));
        Physics2D.queriesHitTriggers = prevQueriesHitTriggers;
        return !hit;
        /*
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, (Vector2)transform.position + dir.normalized * dist, LayerMask.GetMask("Default"));
        foreach (RaycastHit2D hit in hits)
        {
            bool isTrigger = hit.collider.isTrigger;
            Actor hitActor = hit.collider.GetComponentInParent<Actor>();
            bool isActor = hitActor != null;
            bool isSkill = hit.collider.GetComponentInParent<Skill>() != null;

            if (!isTrigger && !isActor && !isSkill)
            {
                return false;
            }
        }
        */
        
        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var hitActor = collision.gameObject.GetComponent<Actor>();
        if (hitActor == null) return;
        if(MoveController.IsDodging)
        {
            var vel = -collision.relativeVelocity.normalized;
            //hitActor.Body.AddDecayForce(vel, vel.magnitude);
            hitActor.MoveController.ApplyKnockback(0.75f);
            hitActor.Body.AddForce(vel, ForceMode2D.Impulse);
            MoveController.MoveDir *= 0.5f;
        }
    }

    #region Container
    bool _inPortal;
    public void SetInPortal(bool inPortal)
    {
        this._inPortal = inPortal;
    }
    void ScaleUpdate()
    {
        float targetS = _initScale;
        if (_inPortal) targetS *= 0.5f;
        if (IsInUI) targetS *= 5f;

        float lerpS = Mathf.Lerp(transform.localScale.x, targetS, 12f * Time.deltaTime);
        transform.localScale = lerpS * Vector3.one;
    }
    #endregion
}
