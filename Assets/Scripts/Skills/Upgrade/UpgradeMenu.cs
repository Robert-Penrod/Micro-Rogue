using UnityEngine;

public class UpgradeMenu : PersistantSingleton<UpgradeMenu>
{
    [Header("Params")]
    [SerializeField] float _lerpSpeed = 12f;

    [Header("References")]
    [SerializeField] SpriteRenderer _upgradeBgSprite;

    bool _isOpen;

    private void Update()
    {
        float lerpAlph = _upgradeBgSprite.color.a.Lerp(_isOpen ? 1f : 0f, _lerpSpeed * Time.deltaTime);
        //_upgradeBgSprite.color = _upgradeBgSprite.color.Alpha(lerpAlph);
    }

    public void DoUpgradeMenuFor(Actor actor)
    {
        SetMenuOpen(true);
    }

    void SetMenuOpen(bool isOpen)
    {
        CameraManager.Instance.ZoomKnob = isOpen ? 0.75f : 1f;
        _isOpen = isOpen;
    }
}
