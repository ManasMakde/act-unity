using UnityEngine;


public class Web : ProjectileBase
{
    // Private Properties
    [SerializeField] private GameObject stickyEffectPrefab;

    
    // Override Methods
    protected override void HitBehaviour(Collider2D other)
    {
        // Return if not trappable
        ITrappable trappable = other.GetComponent<ITrappable>();
        if (trappable == null)
        {
            return;
        }


        // Apply sticky effect
        if (stickyEffectPrefab != null)
        {
            Instantiate(stickyEffectPrefab, other.transform.position, Quaternion.identity, other.transform);
        }
    }
}
