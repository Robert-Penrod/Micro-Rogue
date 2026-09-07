using UnityEngine;

public class HUDUI : MonoBehaviour
{
    #region Vars
    [SerializeField] CanvasGroup _mainGroup;
    [SerializeField] CanvasGroup _dungeonInfoGroup;
    bool _toggle = true;
    bool _isInRun => (DungeonManager.I?.Data?.Coordinate.y ?? 0) > 0;
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
        _mainGroup.alpha = _toggle ? 1f : 0f;
        _dungeonInfoGroup.alpha = _toggle && _isInRun ? 1f : 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _toggle = !_toggle;
            UpdateUI();
        }
    }
}
