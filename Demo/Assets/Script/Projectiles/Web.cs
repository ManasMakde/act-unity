using UnityEngine;

public class Web : ProjectileBase
{
    [SerializeField] private GameObject stickyEffectPrefab;

    protected override void HitBehaviour(Collider2D other)
    {
        // Restart counter if already sticky
        StickyEffect existingSticky = other.GetComponentInChildren<StickyEffect>();
        if(existingSticky != null)
        {
            existingSticky.Restart();
            return;
        }


        // Return if not trappable
        ITrappable trappable = other.GetComponent<ITrappable>();
        if (trappable == null)
        {
            return;
        }


        // Apply sticky effect
        if (stickyEffectPrefab == null)
        {
            return;
        }
        Instantiate(stickyEffectPrefab, other.transform.position, Quaternion.identity, other.transform);
    }
}
