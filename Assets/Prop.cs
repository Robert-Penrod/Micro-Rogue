using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] bool _blocksSight = true;

    [SerializeField] float _health = -1f;
    [SerializeField] float _minSizeMult = 0.8f;
    [SerializeField] float _maxSizeMult = 1.2f;

    private void Start()
    {
        var scale = transform.localScale * Random.Range(_minSizeMult, _maxSizeMult);
        scale.x *= Random.value < 0.5f ? 1 : -1;
        scale.z = 1f;
        transform.localScale = scale;
    }
}
