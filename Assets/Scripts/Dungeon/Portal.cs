using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int GemCost = 0;
    public bool IsEnterable => GemCost <= Player.PlayerData.Gem;

    public int Level = -1;
    float _targetScale = 0.8f;

    [SerializeField] bool _sampleDataOnStart = false;
    
    [SerializeField] GameObject _eliteIcon;
    [SerializeField] GameObject _elite2Icon;
    [SerializeField] GameObject _bossIcon;
    [SerializeField] GameObject _finalBossIcon;
    [SerializeField] TextMeshPro _textMesh;
    [SerializeField] TextMeshPro _lvlText;
    [SerializeField] TextMeshPro _costText;
    [SerializeField] SpriteRenderer _biomeIcon;
    [SerializeField] SpriteRenderer _lockIcon;

    [SerializeField] AudioClip _enterSound;
    [SerializeField] AudioClip _exitSound;
    float _grabForce = 2f;
    List<Player> _grabbedPlayers = new();
    Dictionary<Player, float> _leaveLog = new();

    [SerializeField] Transform _portalGFX;
    List<SpriteRenderer> _spriteRends = new();
    Dictionary<SpriteRenderer, Color> _initColor = new();
    float _baseScale = 0.8f;

    float _lerpRot;

    [field: SerializeField] public DungeonManager.DungeonData DungeonData { get; private set; }

    private void Start()
    {
        // Coordinate text
        this.DelayedInvoke(-1, () => this._textMesh.text = $"{DungeonData.Coordinate.x}, {DungeonData.Coordinate.y}");

        // Sprite Rends
        _spriteRends = new(_portalGFX.GetComponentsInChildren<SpriteRenderer>());
        _spriteRends.ForEach(spriteRend =>
        {
            _initColor.Add(spriteRend, spriteRend.color);
        });

        // Init on Start
        if(_sampleDataOnStart)
        {
            SetData(new DungeonManager.DungeonData(DungeonData.Seed, DungeonData.Coordinate, DungeonData.RunTier));
        }
        else
        {
            SetData(DungeonData);
        }
    }

    public void UpdateUI()
    {
        SetData(DungeonData);
    }

    public void SetData(DungeonManager.DungeonData data)
    {
        // Init
        this.DungeonData = data;
        this._textMesh.text = $"{DungeonData.Coordinate.x}, {DungeonData.Coordinate.y}";

        // Biome
        _biomeIcon.sprite = DungeonManager.I.GetBiomeSprite(DungeonData.Biome);
        _biomeIcon.color = DungeonManager.I.GetBiomeColor(DungeonData.Biome).Alpha(_biomeIcon.color.a);
        _biomeIcon.gameObject.SetActive(_biomeIcon.sprite != null);

        // Level
        Level = DungeonData.Coordinate.y - DungeonManager.I.Data.Coordinate.y;

        // Level Color
        Color lvlColor = Level switch
        {
            1 => Color.white,
            2 => Color.HSVToRGB(0.62f, 0.7f, 1f), // 125 starting = 0.49
            3 => Color.HSVToRGB(0.78f, 0.7f, 1f),
            _ => Color.red
        };

        // Level Difficulty
        // boss
        int bossPeriod = 5;
        int currentLevel = DungeonManager.I.Data.RoomNumber;
        int nextLevel = DungeonData.Coordinate.y;
        int levelstoBoss = bossPeriod - (currentLevel % bossPeriod);
        int levelsGained = nextLevel - currentLevel;
        if(levelsGained >= levelstoBoss)
        {
            this.DungeonData.IsBoss = true;
        }
        if(Level > 1)
        {
            //this.DungeonData.EliteTier = 1;
        }

        // Level Text
        _lvlText.gameObject.SetActive(Level != 1 && IsEnterable);
        _lvlText.text = $"+{Level}";

        // Cost Text
        _costText.gameObject.SetActive(GemCost > 0);
        _costText.text = "-" + GemCost.ToString();

        // Sprites
        _spriteRends.ForEach(spriteRend =>
        {
            if (Level == 1) spriteRend.color = _initColor[spriteRend];
            else
            {
                spriteRend.color = lvlColor.Alpha(spriteRend.color.a);
            }
        });

        // Size
        _baseScale = 0.8f * Level.Remap(1f, 2f, 1f, 1.25f);

        // Icons
        _eliteIcon.SetActive(DungeonData.EliteTier == 1);
        _eliteIcon.transform.localScale = Vector3.one * DungeonData.EliteTier.Remap(1f, 2f, 1f, 1.2f);
        _elite2Icon.SetActive(DungeonData.EliteTier == 2);
        _elite2Icon.transform.localScale = Vector3.one * DungeonData.EliteTier.Remap(1f, 2f, 1f, 1.2f);
        _bossIcon.SetActive(!DungeonData.IsFinalBoss && DungeonData.IsBoss);
        _finalBossIcon.SetActive(DungeonData.IsFinalBoss);
    }

    private void Update()
    {
        // Lock
        _lockIcon.gameObject.SetActive(!IsEnterable);
        UpdateUI();

        float scale = _baseScale;
        float targetRot = 0f;
        if (this == DungeonManager.I.SelectedPortal)
        {
            // Scale
            scale *= DungeonManager.I.PortalPercent.RemapPercent(1f, 1.375f); // PortalPercent mult
            
            // Rotation
            targetRot = 1.5f * Time.deltaTime * 360f * DungeonManager.I.PortalPercent;
        }

        // Scale
        scale *= PlayerManager.I.PlayerList.Count > 0 ? ((float)_grabbedPlayers.Count / PlayerManager.I.PlayerList.Count).RemapPercent(1f, 1.375f) : 1;
        scale *= IsEnterable ? 1f : 0.75f;
        float lerpScale = transform.localScale.x.Lerp(scale, 3f * Time.deltaTime);
        transform.localScale = Vector3.one * lerpScale;

        // Rotation
        _lerpRot = _lerpRot.Lerp(targetRot, 6f * Time.deltaTime);
        transform.GetChild(0).Rotate2D(_lerpRot);
    }

    private void FixedUpdate()
    {
        if (IsEnterable)
        {
            for (int i = 0; i < _grabbedPlayers.Count; i++)
            {
                // Info
                float dist = Vector2.Distance(transform.position, _grabbedPlayers[i].Actor.transform.position);

                // Grab Force
                float distMult = dist.Remap(0.125f, 1f, 0f, 1f);
                Vector2 towardsCore = transform.position - _grabbedPlayers[i].Actor.transform.position;
                Vector2 grabForceVector = distMult * _grabForce * towardsCore.normalized;
                _grabbedPlayers[i].Actor.Body.AddForce(grabForceVector * _grabbedPlayers[i].Actor.Body.linearDamping);

                // Slide Force
                _grabbedPlayers[i].Actor.Body.AddForce(0.5f * _grabbedPlayers[i].Actor.Body.linearVelocity);

                // Dampening
                _grabbedPlayers[i].Actor.Body.linearVelocity = _grabbedPlayers[i].Actor.Body.linearVelocity * (1f - (2f * Time.fixedDeltaTime));
            }
        }
        else
        {
            for (int i = 0; i < _grabbedPlayers.Count; i++)
            {
                Vector2 dir = _grabbedPlayers[i].Actor.transform.position - transform.position;
                Vector2 forceVector = dir.normalized * 5f * dir.magnitude.Remap(0f, 1f, 1f, 0.75f);

                // Slide Force
                _grabbedPlayers[i].Actor.Body.AddDampForce(forceVector);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check Tag
        if (!collision.CompareTag("Player")) 
            return;

        // Check Player 
        var player = collision.GetComponentInParent<Player>();
        if (player == null || _grabbedPlayers.Contains(player))
            return;

        // Check leaveLog
        if (_leaveLog.ContainsKey(player))
        {
            if (Time.time - _leaveLog[player] < 0.2f) return;
        }

        // Add player
        _grabbedPlayers.Add(player);
        player.Actor.SetInPortal(true);

        HandlePlayerEnter(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var actor = collision.GetComponent<Actor>();
        if (actor == null) return;
        var player = actor.GetComponentInParent<Player>();
        if (player != null && _grabbedPlayers.Contains(player))
        {
            _grabbedPlayers.Remove(player);

            // Log
            if (_leaveLog.ContainsKey(player)) _leaveLog[player] = Time.time;
            else _leaveLog.Add(player, Time.time);

            player.Actor.SetInPortal(false);

            // Kick
            Vector2 kickVector = ((Vector2)(player.Actor.transform.position - transform.position)).normalized;
            kickVector *= 0.125f * _grabForce;
            player.Actor.Body.AddForce(kickVector * player.Actor.Body.linearDamping, ForceMode2D.Impulse);
            Debug.DrawLine(transform.position, transform.position + (Vector3)kickVector, Color.green, 1f);

            HandlePlayerExit(player);
        }
    }

    void HandlePlayerEnter(Player player)
    {
        if (!IsEnterable) return; 
        player.SelectedPortal = this;
        AudioSpawner.PlayAudioWithRandPitch(_enterSound, 0.2f, 1.25f, 1f);
    }

    void HandlePlayerExit(Player player)
    {
        player.SelectedPortal = null;
        if (!IsEnterable) return;
        AudioSpawner.PlayAudioWithRandPitch(_exitSound, 0.2f, 0.75f, 1f);
    }
}
