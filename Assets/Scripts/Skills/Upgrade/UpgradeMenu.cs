using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeMenu : Singleton<UpgradeMenu>
{
    [Header("Params")]
    [SerializeField] float _lerpSpeed = 12f;

    [Header("References")]
    [SerializeField] Transform _actorPlatform;
    [SerializeField] GameObject _rarityFlipGFX;
    [SerializeField] SimpleButton_Old _rerollButton;
    [SerializeField] GameObject _toggleObjects;
    List<SpriteRenderer> _upgradeBgSprite = new();
    Dictionary<SpriteRenderer, float> _upgradeBgAlphaInit = new();
    CanvasGroup _canvasGroup;
    [SerializeField] List<UpgradeCardUI> _upgradeCardList = new();

    public bool IsOpen { get; private set; }
    public Actor _actorToUpgrade;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _upgradeBgSprite.AddRange(GetComponentsInChildren<SpriteRenderer>());
        _upgradeBgSprite.ForEach(sprite =>
        {
            _upgradeBgAlphaInit.Add(sprite, sprite.color.a);
            sprite.color = sprite.color.Alpha(0f);
        });
        SetMenuOpen(false);
    }

    private void Update()
    {
        // Dungeon Veil Alpha
        _upgradeBgSprite.ForEach(sprite =>
        {
            sprite.color = sprite.color.Alpha(sprite.color.a.Lerp(IsOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime));
        });

        // Canvas Alpha
        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(IsOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);

        // Testing
        if(Input.GetKeyDown(KeyCode.R))
        {
            RollUpgradeCards();
        }
    }

    public void DoUpgradeMenuFor(Actor actor)
    {
        this._actorToUpgrade = actor;
        RollUpgradeCards();
        SetMenuOpen(true);
        
        if(actor.IsPlayer())
        {
            var player = actor.GetComponentInParent<Player>();
            PlayerManager.I.SetUIOwner(player);
        }
    }

    void RollUpgradeCards()
    {
        //Debug.Log("Rerolling");
        float rarityFlip = (DungeonManager.I.PreviousData?.EliteTier > 0) ? 5f : 0f; 
        float newSkillMult = (DungeonManager.I.PreviousData?.EliteTier > 0) ? 5f : 1f;
        _rarityFlipGFX.SetActive(rarityFlip > 0);
        SetUpgradeOptions(UpgradeManager.I.GetUpgradeOptions(_actorToUpgrade, rarityFlip: rarityFlip, newSkillMult: newSkillMult));

        bool canReroll = ActorCanReroll(_actorToUpgrade);
        _rerollButton.gameObject.SetActive(canReroll);

        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

        if (currentSelectedGameObject == null || (!canReroll && currentSelectedGameObject == _rerollButton.gameObject))
        {
            EventSystem.current.SetSelectedGameObject(_upgradeCardList[0].gameObject);
        }
    }

    public void BuyReroll()
    {
        if(_actorToUpgrade.IsPlayer())
        {
            var player = _actorToUpgrade.GetComponentInParent<Player>();
            player.Data.Coin -= 5;
        }

        RollUpgradeCards();
    }

    public bool ActorCanReroll(Actor actor)
    {
        if (actor == null) return false;

        if(actor.IsPlayer())
        {
            var player = actor.GetComponentInParent<Player>();
            return player.Data.Coin >= 5;
        }

        return false;
    }

    public void SetMenuOpen(bool isOpen)
    {
        CameraManager.I.Zoom(isOpen ? 1.1f : 1f, this);
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = isOpen;
        IsOpen = isOpen;

        foreach(var particleSystem in GetComponentsInChildren<ParticleSystem>(true))
        {
            if(isOpen)
            {
                //particleSystem.Stop();
                //particleSystem.Play();
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();
                Debug.Log("Open Menu");
            }
            else
            {
                //particleSystem.Stop();
                particleSystem.gameObject.SetActive(false);
                particleSystem.Stop();
                Debug.Log("Close Menu");
            }
        }

        if(!isOpen)
        {
            PlayerManager.I.SetUIOwner(null);
            EventSystemSingleton.I.ClearSelection();
            PlayerManager.I.PlayerList.ForEach(player =>
            {
                player.Actor.SetInUI(false);
            });
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(_upgradeCardList[0].gameObject);

            PlayerManager.I.PlayerList.ForEach(player =>
            {
                player.Actor.SetInUI(true);
                var actorTransform = player.Actor.transform;
                if (_actorToUpgrade != null && _actorToUpgrade == player.Actor)
                {
                    actorTransform.position = _actorPlatform.position.x * Vector3.right + Vector3.forward * actorTransform.position.z;
                }
                else
                {
                    var pos = 20f * Vector3.right + Vector3.forward * actorTransform.position.z;
                    pos += 5f * (Vector3)Random.insideUnitCircle;
                    actorTransform.position = pos;
                }
            });
        }
    }

    void SetUpgradeOptions(List<Upgrade> upgradeList)
    {
        for(int i = 0; i < _upgradeCardList.Count; i++)
        {
            if (upgradeList != null && i < upgradeList.Count)
            {
                _upgradeCardList[i].gameObject.SetActive(true);
                _upgradeCardList[i].LoadUpgradeData(upgradeList[i]);
            }
            else
            {
                _upgradeCardList[i].gameObject.SetActive(false);
            }
        }
    }
}
