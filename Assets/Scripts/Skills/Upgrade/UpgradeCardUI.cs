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
    [SerializeField] TextMeshProUGUI _slotText;
    [SerializeField] Transform _tagHolder;
    Upgrade _upgrade;

    public void LoadUpgradeData(Upgrade upgrade)
    {
        //Debug.Log(" of " + upgrade._sourceSkill.Name);
        // Init
        var skillColor = upgrade.GetColor().Lerp(Color.white, 0.25f);
        var rarityColor = Constants.RarityToColor(upgrade.Rarity);
        var upgradeColor = upgrade.Rarity != Constants.Rarity.Common ? rarityColor : skillColor;
        this._upgrade = upgrade;

        // Card
        _frame.color = upgradeColor;
        _bg.color = upgradeColor.SetValue(_bg.color.GetValue());

        // Icon
        this._icon.sprite = upgrade.GetIcon();
        this._icon.color = skillColor;
        _iconGlow.color = skillColor.Alpha(_iconGlow.color.a);

        // Title
        this._title.text = upgrade.GetTitle();
        this._title.color = upgradeColor;

        // Slot
        this._slotText.text = upgrade.GetSlot();

        // Lvl
        this._level.text = upgrade.GetLevel();

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
