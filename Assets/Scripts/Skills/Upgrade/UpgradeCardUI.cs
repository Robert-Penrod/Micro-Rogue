using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _level;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] Transform _tagHolder;
    Upgrade _upgrade;

    public void LoadUpgradeData(Upgrade upgrade)
    {
        this._upgrade = upgrade;

        this._icon = null;
        this._title.text = upgrade.GetTitle();
        this._level.text = "NEW";
        this._description.text = upgrade.ToString();
        // tags
    }
}
