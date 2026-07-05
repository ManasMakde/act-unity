using UnityEngine;

public class Web : ProjectileBase
{
    [SerializeField] private GameObject webEffectPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Return if not trappable
        ITrappable trappable = other.GetComponent<ITrappable>();
        if (trappable == null)
        {
            return;
        }


        // Trap in web
        if (webEffectPrefab == null)
        {
            return;
        }
        Instantiate(webEffectPrefab, other.transform.position, Quaternion.identity, other.transform);
        Destroy(gameObject);
    }
}
