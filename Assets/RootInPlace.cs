using UnityEngine;

public class RootInPlace : MonoBehaviour
{
    #region Vars
    [SerializeField] float _force;

    Vector2 _initPos;
    Rigidbody2D _rb;
    #endregion

    #region Init
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        _initPos = transform.position;
    }
    #endregion

    #region Update
    private void FixedUpdate()
    {
        Vector2 targetDir = _initPos - _rb.position;
        float dist = targetDir.magnitude;
        targetDir.Normalize();

        Vector2 forceVector = _force * targetDir * dist.Remap(0f, 1f, 0f, 1f);

        _rb.AddDampForce(forceVector);
    }
    #endregion
}
