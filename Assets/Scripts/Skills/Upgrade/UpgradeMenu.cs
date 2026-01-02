using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeMenu : PersistantSingleton<UpgradeMenu>
{
    [Header("Params")]
    [SerializeField] float _lerpSpeed = 12f;

    [Header("References")]
    [SerializeField] SimpleButton _rerollButton;
    [SerializeField] SpriteRenderer _upgradeBgSprite;
    CanvasGroup _canvasGroup;
    [SerializeField] List<UpgradeCardUI> _upgradeCardList = new();

    public bool IsOpen { get; private set; }
    Actor _actorToUpgrade;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetMenuOpen(false);
    }

    private void Update()
    {
        // Dungeon Veil Alpha
        float lerpAlph = _upgradeBgSprite.color.a.Lerp(IsOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);
        _upgradeBgSprite.color = _upgradeBgSprite.color.Alpha(lerpAlph);

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
        SetUpgradeOptions(UpgradeManager.I.GetUpgradeOptions(_actorToUpgrade));
        _rerollButton.gameObject.SetActive(ActorCanReroll(_actorToUpgrade));
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
        if(actor.IsPlayer())
        {
            var player = actor.GetComponentInParent<Player>();
            return player.Data.Coin >= 5;
        }

        return false;
    }

    public void SetMenuOpen(bool isOpen)
    {
        CameraManager.I.ZoomKnob = isOpen ? 1.1f : 1f;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = isOpen;
        IsOpen = isOpen;

        if(!isOpen)
        {
            PlayerManager.I.SetUIOwner(null);
            EventSystem.current.SetSelectedGameObject(null);

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
                    actorTransform.position = 16f * Vector3.right + Vector3.forward * actorTransform.position.z;
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
