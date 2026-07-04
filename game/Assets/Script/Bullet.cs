using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float lifeSpan = 5.0f;  // In Seconds
    [SerializeField] public float speed = 20f;
    [SerializeField] public Vector2 direction = new();
    private Rigidbody2D rb;

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Overlapped");
    }
}
