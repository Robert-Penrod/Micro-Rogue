using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonInfoUI : MonoBehaviour
{
    [SerializeField] Image _eliteimage;
    [SerializeField] Image _bossImage;
    [SerializeField] TextMeshProUGUI _nextTierText;
    [SerializeField] TextMeshProUGUI _currentTierText;

    private void Start()
    {
        UpdateUI();
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            UpdateUI();
        };
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Boss/Elite Images
        _eliteimage.enabled = DungeonManager.I?.Data?.EliteTier > 0;
        _bossImage.enabled = DungeonManager.I?.Data?.IsBoss ?? false;

        // Tier Text
        _nextTierText.text = Utils.ToRomanNumeral((DungeonManager.I?.Data?.RunTier + 1) ?? 0);
        _currentTierText.text = Utils.ToRomanNumeral(DungeonManager.I?.Data?.RunTier ?? 0);
    }
}
