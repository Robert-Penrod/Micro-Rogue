using UnityEngine;

public class XpMeter : MonoBehaviour
{
    [SerializeField] Transform _fill;
    [SerializeField] SimpleMenu _nodeMenu;
    CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        float targetAlpha = 0f;
        if (DungeonManager.I.IsRunStarted || _nodeMenu.IsOpen) targetAlpha = 1f;
        float lerpAlpha = _canvasGroup.alpha.Lerp(targetAlpha, 12f * Time.deltaTime);
        _canvasGroup.alpha = lerpAlpha;

        float targetFill = Player.PlayerData.XpPercent;
        float lerpFill = _fill.transform.localScale.x.Lerp(targetFill, 12f * Time.deltaTime);
        _fill.transform.localScale = new Vector3(lerpFill, 1f, 1f);
    }
}
