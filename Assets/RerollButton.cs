using TMPro;
using UnityEngine;

public class RerollButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _costText;

    private void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        _costText.text = "-" + (UpgradeMenu.I?.GetUpgradeCost() ?? 5).ToString();
    }
}
