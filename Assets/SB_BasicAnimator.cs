using UnityEngine;

[RequireComponent(typeof(SimpleButton))]
public class SB_BasicAnimator : MonoBehaviour
{
    [SerializeField] float _lerpSpeed = 12f;

    [Header("Size")]
    [SerializeField] float _selectSizeMult = 1f;
    [SerializeField] float _pressSizeMult = 1f;
    [SerializeField] float _hoverSizeMult = 1f;

    SimpleButton _simpleButton;

    private void Awake()
    {
        _simpleButton = GetComponent<SimpleButton>();
    }

    private void Update()
    {
        // Scale
        {
            float targetScale = 1f;
            if (_simpleButton.IsHovered) targetScale *= _hoverSizeMult;
            if (_simpleButton.IsSelected) targetScale *= _selectSizeMult;
            targetScale *= _simpleButton.PressPercent.RemapPercent(1f, _pressSizeMult, false);
            float lerp = _lerpSpeed;
            lerp *= targetScale > transform.localScale.x ? 2f : 1f;
            transform.localScale = transform.localScale.Lerp(Vector3.one * targetScale, lerp * Time.deltaTime);
        }
    }
}
