using UnityEngine;

public class VenomEffect : MonoBehaviour
{
    // Private Properties
    [SerializeField] private float damageDelay = 1f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float damageDuration = 4f;
    [SerializeField] private float damageAmount = 2f;
    private IDamageable target;


    // Private Methods
    private void DamageTarget()
    {
        target.TakeDamage(damageAmount);
    }
    private void EndEffect()
    {
        CancelInvoke(nameof(DamageTarget));
        Destroy(gameObject);
    }


    // Override Methods
    private void Start()
    {
        // Return if no target to damage
        target = GetComponentInParent<IDamageable>();
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }


        // Keep damaging in intervals
        InvokeRepeating(nameof(DamageTarget), damageDelay, damageInterval);


        // Stop damaging
        Invoke(nameof(EndEffect), damageDuration);
    }
}
