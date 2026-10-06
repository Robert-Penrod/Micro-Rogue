using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineCanvasAlpha : MonoBehaviour
{
    LineRenderer _lineRend;
    CanvasGroup _canvasGroup;

    private void Start()
    {
        _lineRend = GetComponent<LineRenderer>();
        _canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    private void Update()
    {
        if (_canvasGroup == null) return;

        SetAlpha(_canvasGroup.alpha);
    }

    void SetAlpha(float alpha)
    {
        _lineRend.startColor = _lineRend.startColor.Alpha(alpha);
        _lineRend.endColor = _lineRend.endColor.Alpha(alpha);
    }
}
