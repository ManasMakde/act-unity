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
    private System.Type[] ignoreList;


    // Public Methods
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }
    public void SetIgnoreList(params System.Type[] newIgnoreList)
    {
        ignoreList = newIgnoreList;
    }


    // Protected
    protected virtual void HitBehaviour(Collider2D other) { }


    // Private Methods
    private bool HasIgnoredComponent(Collider2D other)
    {
        // Return false if list not set
        if (ignoreList == null)
        {
            return false;
        }


        // Check each ignore type against hit object
        foreach (System.Type ignoreType in ignoreList)
        {
            if (ignoreType == null)
            {
                continue;
            }
            if (other.GetComponent(ignoreType) != null)
            {
                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Return if to ignore owner
        if (other.gameObject == owner && toIgnoreOwner)
        {
            return;
        }


        // Return if hit object has an ignored component
        if (HasIgnoredComponent(other))
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
