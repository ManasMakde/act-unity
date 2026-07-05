using UnityEngine;

public class WebEffect : MonoBehaviour
{
    // Private Properties
    [SerializeField] private float trapDuration = 3f;
    private ITrappable target;


    // Private Methods
    private void UntrapAndDestroy()
    {
        target.Untrap();
        Destroy(gameObject);
    }


    // Override Methods
    private void Start()
    {
        target = GetComponentInParent<ITrappable>();
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Trap Target
        target.Trap(trapDuration);
        Invoke(nameof(UntrapAndDestroy), trapDuration);
    }

}
