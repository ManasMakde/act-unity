using UnityEngine;
using System.Collections;

public class ProjectileBase : MonoBehaviour
{
    // Public Methods
    [SerializeField] public float lifeSpan = 3.0f;  // In Seconds
    [SerializeField] public float speed = 20f;
    [SerializeField] public bool toIgnoreOwner = true;
    [SerializeField] public Vector2 direction = new();
    public GameObject owner { private set; get; } = null;


    // Private Methods
    private Rigidbody2D rb;


    // Public Methods
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }


    // Protected
    protected virtual void HitBehaviour(Collider2D other)
    {
    }


    // Private Methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Return if to ignore owner
        if (other.gameObject == owner && toIgnoreOwner)
        {
            return;   
        }


        // For child classes
        HitBehaviour(other);


        // Destroy other if projectile
        ProjectileBase otherProjectile = other.GetComponent<ProjectileBase>();
        if (otherProjectile != null)
        {
            Destroy(otherProjectile.gameObject);
        }


        // Destroy Self
        Destroy(gameObject);
    }


    // Override Methods
    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Destroy(gameObject, lifeSpan);
    }
}
