using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    // Public Methods
    [SerializeField] public float lifeSpan = 3.0f;  // In Seconds
    [SerializeField] public float speed = 20f;
    [SerializeField] public Vector2 direction = new();
    public GameObject owner { private set; get; } = null;


    // Private Methods
    private Rigidbody2D rb;


    // Public Methods
    void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }


    // Override Methods
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Destroy(gameObject, lifeSpan);
    }
    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }
}
