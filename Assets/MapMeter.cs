using UnityEngine;

public class MapMeter : MonoBehaviour
{
    [SerializeField] Transform _fill;

    [SerializeField] float _lerpSpeed = 12f;
    float _targetPercent;

    private void Start()
    {
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            _targetPercent = DungeonManager.I.GetLoopPercent();
            //Debug.Log("Updating Percent " + _targetPercent);

            RectTransform rectTransform = GetComponent<RectTransform>();
            float height = 0.75f * 5f * 16f * DungeonManager.I.Data.RunTier;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        };
    }

    Vector3 _s;
    private void Update()
    {

        _s = _fill.transform.localScale;
        _s.y = _s.y.Lerp(_targetPercent, _lerpSpeed * Time.deltaTime);
        _fill.transform.localScale = _s;
    }
}
