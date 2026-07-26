using System;
using UnityEngine;


public class StickyEffect : MonoBehaviour
{
    // Public Actions
    public event Action OnEffectEnd;


    // Private Properties
    [SerializeField] private float trapDuration = 3f;
    private ITrappable target;
    private float lockedZRotation;
    private bool isEnding = false;  // Marked true right before this sticky untraps or bails


    // Public Methods
    public void Restart()
    {
        CancelInvoke(nameof(UntrapAndDestroy));
        target?.Trap(trapDuration);
        Invoke(nameof(UntrapAndDestroy), trapDuration);
    }


    // Private Methods
    private void UntrapAndDestroy()
    {
        // Mark self as ending before checking siblings
        isEnding = true;


        // Skip untrap if another non ending sticky exists on target
        StickyEffect[] siblingStickies = transform.parent.GetComponentsInChildren<StickyEffect>();
        foreach (StickyEffect sticky in siblingStickies)
        {
            if (sticky != this && !sticky.isEnding)
            {
                Destroy(gameObject);
                return;
            }
        }


        target.Untrap();
        OnEffectEnd?.Invoke();  // Broadcast before destroy so listeners can react
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


        // To avoid rotating with player
        lockedZRotation = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, lockedZRotation);


        // Trap Target
        target.Trap(trapDuration);
        Invoke(nameof(UntrapAndDestroy), trapDuration);
    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, lockedZRotation); // Not the best way, someone should refactor
    }

}
