using UnityEngine;

public class Bullet : ProjectileBase
{
    // Public Methods
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] public bool damageOwner = false;

    protected override void HitBehaviour(Collider2D other)
    {
        // Return if cannot damage owner
        if (other.gameObject == owner && !damageOwner)
        {
            return;   
        }


        // Return if no Damage Interface
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return;
        }


        // Apply damage
        damageable.TakeDamage(damageAmount);
        // Destroy(gameObject);
    }
}
