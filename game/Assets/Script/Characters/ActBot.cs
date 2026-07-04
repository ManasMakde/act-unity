using UnityEngine;

public class ActBot : MonoBehaviour
{
    // Act Properties
    private Theater theater;
    [SerializeField] public MoveAct moveAct = new();
    [SerializeField] public ShootAct shootAct = new();


    // Override Methods
    void Update()
    {
        // Move
        moveAct.direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;


        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shootAct.bulletDirection = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
            shootAct.Perform();
        }
    }
    void FixedUpdate()
    {
        moveAct.Perform();
    }
    void Start()
    {
        // Setup acts
        theater = GetComponent<Theater>();
        moveAct.Init(theater, "Move Act");
        shootAct.Init(theater, "Shoot Act");
    }
}
