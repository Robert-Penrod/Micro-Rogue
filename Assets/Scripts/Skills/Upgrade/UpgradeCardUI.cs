using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image _icon;
    [SerializeField] Image _iconGlow;
    [SerializeField] Image _frame;
    [SerializeField] Image _bg;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _level;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] Transform _tagHolder;
    Upgrade _upgrade;

    public void LoadUpgradeData(Upgrade upgrade)
    {
        // Init
        var upgradeColor = upgrade.GetColor().Lerp(Color.white, 0.25f);
        this._upgrade = upgrade;

        // Icon
        this._icon.sprite = upgrade.GetIcon();
        this._icon.color = upgradeColor;
        _iconGlow.color = upgradeColor.Alpha(_iconGlow.color.a);

        // Title
        this._title.text = upgrade.GetTitle();
        this._title.color = upgradeColor;

        // Card
        _frame.color = upgradeColor;
        _bg.color = upgradeColor.SetValue(_bg.color.GetValue());

        // Lvl
        this._level.text = "NEW";

        // Description
        this._description.text = upgrade.GetDescription();

        // Tags
    }

    public void ChooseCard()
    {
        _upgrade.ApplyUpgrade();
        UpgradeManager.I.FinishUpgrading();
    }
}
