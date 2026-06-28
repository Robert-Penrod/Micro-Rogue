using UnityEngine;

public class HUDUI : MonoBehaviour
{
    #region Vars
    [SerializeField] CanvasGroup _dungeonInfoGroup;
    #endregion

    #region Init
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
    #endregion

    void UpdateUI()
    {
        bool isInRun = (DungeonManager.I?.Data?.Coordinate.y ?? 0) > 0;
        _dungeonInfoGroup.alpha = isInRun ? 1f : 0f;
    }
}
