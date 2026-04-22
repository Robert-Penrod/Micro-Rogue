using UnityEngine;
using UnityEngine.UI;

public class DungeonInfoUI : MonoBehaviour
{
    [SerializeField] Image _eliteimage;
    [SerializeField] Image _bossImage;

    private void Start()
    {
        UpdateUI();
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            UpdateUI();
        };
    }

    void UpdateUI()
    {
        _eliteimage.enabled = DungeonManager.I?.Data?.EliteTier > 0;
        _bossImage.enabled = DungeonManager.I?.Data?.IsBoss ?? false;
    }
}
