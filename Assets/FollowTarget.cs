using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _force;

    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 targetDir = (Vector2)_target.transform.position - _rb.position;
        float dist = targetDir.magnitude;
        targetDir.Normalize();

        Vector2 forceVector = _force * targetDir * dist.Remap(0f, 1f, 0f, 1f);

        _rb.AddDampForce(forceVector);
    }
}
