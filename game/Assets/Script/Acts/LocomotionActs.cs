using UnityEngine;


public class MoveAct : Act
{
    // Private Properties
    public Vector2 direction = new();
    public float speed = 5f;
    public Rigidbody2D rb;


    // Override Methods
    protected override void Setup()
    {
        rb ??= GetOwner().GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;  // No gravity for top down
    }
    protected override bool CanPerform()
    {
        return rb != null;
    }
    protected override Outcome Enter()
    {
        rb.MovePosition(rb.position + direction * speed * GetPhysicsDelta());
        return Outcome.Success;
    }
    protected override void Exit()
    {
        direction = Vector2.zero;
    }
}
