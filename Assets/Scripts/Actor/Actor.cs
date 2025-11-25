using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }
    float _initScale;
    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        _initScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
    }
    
    private void Update()
    {
        ScaleUpdate();
    }

    private void FixedUpdate()
    {
        HandleMoveFixedUpdate();
    }

    #region Move
    public Vector2 MoveInput { get; private set; }
    float _moveTime;
    bool _moveRest;
    [SerializeField] bool _doMoveTime = true;
    public void Move(Vector2 move)
    {
        MoveInput = move;
    }

    void HandleMoveFixedUpdate()
    {
        StepMoveTime();

        float baseSpeed = Constants.ActorStats.MoveSpeed.Default;
        float movetimeMult = _doMoveTime ? _moveTime.Remap(5f, 10f, 1f, 0.8f) : 1f;

        Body.AddForce(movetimeMult * baseSpeed * MoveInput * Body.linearDamping);
    }

    void StepMoveTime()
    {
        if (!_doMoveTime) return;

        float moveSpeed = Body.linearVelocity.magnitude;
        float movePercent = moveSpeed.Remap(0f, 1f, 0f, 1f);

        // Moving
        if (!_moveRest && Body.linearVelocity.magnitude > 1f)
        {
            _moveTime += movePercent * Time.fixedDeltaTime;
            if (_moveTime > 10)
            {
                _moveTime = 10;
                _moveRest = true;
            }
        }
        // Resting
        else
        {
            _moveTime -= 4f * Time.fixedDeltaTime;
            if (_moveTime < 0)
            {
                _moveTime = 0;
                _moveRest = false;
            }
        }
    }
    #endregion

    #region Container
    bool _inPortal;
    public void SetInPortal(bool inPortal)
    {
        this._inPortal = inPortal;
    }
    void ScaleUpdate()
    {
        float targetS = _initScale;
        if (_inPortal) targetS *= 0.5f;

        float lerpS = Mathf.Lerp(transform.localScale.x, targetS, 12f * Time.deltaTime);
        transform.localScale = lerpS * Vector3.one;
    }
    #endregion

    public bool IsEnemyOf(Actor otherActor)
    {
        return true;
    }
}
