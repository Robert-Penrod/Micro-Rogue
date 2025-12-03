using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumTracker : MonoBehaviour
{
    [Header("Physics")]
    public float Force = 15f;
    public float Mass = 1f;
    public float MaxDist = 5f;

    [Header("Distance")]
    public float OuterDist = 5f;
    public float CentralDist = 0f;

    [Header("Drag")]
    public float OuterDrag = 1f;
    public float CentralDrag = 10f;

    [field:SerializeField] public Vector2 MomentumVector { get; private set; }

    Rigidbody2D _probeBody;

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawLine(transform.position, transform.position + (Vector3)MomentumVector);
    }

    private void Awake()
    {
        _probeBody = CreateProbe();
        _probeBody.gameObject.hideFlags = HideFlags.HideInHierarchy;
        /*
        _probeBody.transform.position = transform.position;
        _probeBody.gameObject.name = gameObject.name + " - Momentum Tracker";
        */
    }

    Rigidbody2D CreateProbe()
    {
        GameObject probeObject = new GameObject("Momentum Tracker");
        Rigidbody2D probeBody = probeObject.AddComponent<Rigidbody2D>();
        probeBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        probeBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        probeBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        probeBody.gravityScale = 0f;
        probeBody.mass = Mass;
        return probeBody;
    }

    private void FixedUpdate()
    {
        // Get physics info
        Vector2 towardsTarget = (Vector2)transform.position - _probeBody.position;
        MomentumVector = -towardsTarget;
        float distance = towardsTarget.magnitude;
        towardsTarget.Normalize();

        if(distance > MaxDist)
        {
            _probeBody.position = transform.position - (Vector3)towardsTarget.normalized * MaxDist;
            distance = MaxDist;

        }

        // Change Drag based on distance
        _probeBody.linearDamping = distance.Remap(CentralDist, OuterDist, CentralDrag, OuterDrag);

        // Apply Force
        Vector2 forceVector = towardsTarget * Force * _probeBody.linearDamping * distance.Remap(0f, 1f, 0f, 1f);
        _probeBody.AddForce(forceVector);
    }
}
