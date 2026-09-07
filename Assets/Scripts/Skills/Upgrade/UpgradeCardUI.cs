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
        rarityColor = skillColor.Lerp(rarityColor, 0.675f);
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
        for (int i = 0; i < this._tagHolder.childCount; i++)
        {
            var tagTransform = _tagHolder.GetChild(i);
            var tagList = upgrade.SourceSkill.Tags.GetTagList();
            if (i < tagList.Count && i < 1)
            {
                var tag = tagList[i];
                tagTransform.gameObject.SetActive(true);
                var image = tagTransform.GetComponentInChildren<Image>();
                var text = tagTransform.GetComponentInChildren<TextMeshProUGUI>();
                text.text = tag.ToString();
                image.color = tag switch
                {
                    TagCollection.TagType.Str => GamePaletteManager.I.Palette.StrColor,
                    TagCollection.TagType.Dex => GamePaletteManager.I.Palette.DexColor,
                    TagCollection.TagType.Int => GamePaletteManager.I.Palette.IntColor,
                    _ => Color.grey
                };
                text.color = image.color;
                image.color = image.color.Alpha(0.5f);
            }
            else
            {
                tagTransform.gameObject.SetActive(false);
            }
        }
    }

    public void ChooseCard()
    {
        _upgrade.ApplyUpgrade();
        UpgradeManager.I.FinishUpgrading();
    }
}
