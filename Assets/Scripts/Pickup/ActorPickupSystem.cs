using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorPickupSystem : MonoBehaviour
{
    public float Range = 3f;
    public float PickupRange = 1f;
    public float Force = 5f;

    [HideInInspector] public Actor PickupActor;

    private void Awake()
    {
        PickupActor = GetComponent<Actor>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PickupRange);
    }

    private void FixedUpdate()
    {
        Collider2D[] colArray = Physics2D.OverlapCircleAll(transform.position, Range, LayerMask.GetMask("Pickup"));
        foreach (Collider2D col in colArray)
        {
            var pickup = col.GetComponent<Pickup>();
            if (pickup == null) continue;

            if (pickup.CanPickUp())
            {
                var body = pickup.GetComponent<Rigidbody2D>();
                if (body != null) Attract(body);

                TryCollection(pickup);
            }
        }
    }

    void Attract(Rigidbody2D body)
    {
        Vector2 dir = (Vector2)transform.position - body.position;
        Vector2 attractForce = Force * dir.normalized;
        body.AddDampForce(attractForce);
    }

    void TryCollection(Pickup pickup)
    {
        float dist = Vector2.Distance(transform.position, pickup.transform.position);
        if(dist <= PickupRange)
        {
            pickup.DoPickup(this);
        }
    }
}
