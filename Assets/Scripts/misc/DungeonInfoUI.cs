using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonInfoUI : MonoBehaviour
{
    [SerializeField] Image _eliteimage;
    [SerializeField] Image _bossImage;
    [SerializeField] Image _finalBossImage;
    [SerializeField] TextMeshProUGUI _runTierText;
    [SerializeField] GameObject _loopObj;
    [SerializeField] GameObject _relicObj;
    [SerializeField] TextMeshProUGUI _loopText;

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
        _eliteimage.gameObject.SetActive(_eliteimage.enabled);
        _bossImage.enabled = DungeonManager.I?.Data?.IsBoss ?? false;
        _bossImage.gameObject.SetActive(_bossImage.enabled);
        _finalBossImage.enabled = DungeonManager.I?.Data?.IsFinalBoss ?? false;
        _finalBossImage.gameObject.SetActive(_finalBossImage.enabled);

        // Tier
        _runTierText.text = Utils.ToRomanNumeral((DungeonManager.I?.Data?.RunTier) ?? 0);

        // Loop / Relic
        int runLoopCount = DungeonManager.I?.Data?.RunLoop ?? 0;
        bool hasCollectedRelic = DungeonManager.I?.GetHasCollectedRelic() ?? false;
        _relicObj.SetActive(!hasCollectedRelic);
        _loopObj.SetActive(hasCollectedRelic);
        _loopText.text = runLoopCount.ToString();
    }
}
