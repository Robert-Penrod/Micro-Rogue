using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }
    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 move)
    {
        MoveInput = move;
    }

    private void FixedUpdate()
    {
        Body.AddForce(3f * MoveInput * Body.linearDamping);
    }
}
