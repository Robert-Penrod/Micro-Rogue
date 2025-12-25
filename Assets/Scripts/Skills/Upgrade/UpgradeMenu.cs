using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeMenu : PersistantSingleton<UpgradeMenu>
{
    [Header("Params")]
    [SerializeField] float _lerpSpeed = 12f;

    [Header("References")]
    [SerializeField] SpriteRenderer _upgradeBgSprite;
    CanvasGroup _canvasGroup;
    [SerializeField] List<UpgradeCardUI> _upgradeCardList = new();

    bool _isOpen;
    Actor _actorToUpgrade;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
        SetMenuOpen(false);
    }

    private void Update()
    {
        // Dungeon Veil Alpha
        float lerpAlph = _upgradeBgSprite.color.a.Lerp(_isOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);
        _upgradeBgSprite.color = _upgradeBgSprite.color.Alpha(lerpAlph);

        // Canvas Alpha
        _canvasGroup.alpha = _canvasGroup.alpha.Lerp(_isOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);

        // Testing
        if(Input.GetKeyDown(KeyCode.R))
        {
            RollUpgradeCards(_actorToUpgrade);
        }
    }

    public void DoUpgradeMenuFor(Actor actor)
    {
        this._actorToUpgrade = actor;
        RollUpgradeCards(_actorToUpgrade);
        SetMenuOpen(true);
    }

    void RollUpgradeCards(Actor actor)
    {
        SetUpgradeOptions(UpgradeManager.I.GetUpgradeOptions(actor));
    }

    public void SetMenuOpen(bool isOpen)
    {
        CameraManager.Instance.ZoomKnob = isOpen ? 1.1f : 1f;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = isOpen;
        _isOpen = isOpen;

        if(!isOpen)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(_upgradeCardList[0].gameObject);
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
