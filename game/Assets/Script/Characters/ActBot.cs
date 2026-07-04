using UnityEngine;

public class ActBot : MonoBehaviour
{
    // Public Properties
    [SerializeField] private float moveSpeed = 5f;

    // Act Properties
    private Theater theater;
    private MoveAct moveAct = new();


    // Override Methods
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveAct.direction = new Vector2(moveX, moveY).normalized;
    }
    void FixedUpdate()
    {
        moveAct.Perform();
    }
    void Start()
    {
        // Setup Components
        theater = GetComponent<Theater>();
        
        // Setup acts
        moveAct.speed = moveSpeed;
        moveAct.Init(theater, "Move Act");
    }
}
