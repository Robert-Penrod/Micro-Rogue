using UnityEngine;

public class KnockbackTest : MonoBehaviour
{
    public enum TestType { IMPULSE, DECAYFORCE }
    public TestType Type;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            Run();   
        }
    }

    public void Run()
    {
        var body = GetComponent<Rigidbody2D>();
        Vector2 forceVector = 1f * Vector2.right;

        switch (Type)
        {
            case TestType.IMPULSE:
                body.linearDamping = 6f;
                body.AddForce(forceVector * body.linearDamping, ForceMode2D.Impulse);
                break;

            case TestType.DECAYFORCE:
                body.AddDecayForce(forceVector);
                break;

            default:
                break;
        }
    }
}
