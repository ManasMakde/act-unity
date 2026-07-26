using System;
using UnityEngine;


[RequireComponent(typeof(Theater))]
public class Player : MonoBehaviour, IDamageable, ITrappable
{
    // Public Actions
    public event Action OnKilled;


    // Private Properties
    [SerializeField] HealthSystem healthSystem = new();
    [SerializeField] GameObject barrelObject;


    // Act Properties
    [SerializeField] Theater theater;
    [SerializeField] MoveAct moveAct = new();
    [SerializeField] ShootAct shootAct = new();
    [SerializeField] DamageAct damageAct = new();
    [SerializeField] LookAct lookAct = new();


    // Static Methods
    public static Rect GetBorderFromCamera(Camera camera)
    {
        if (camera == null)
        {
            return new Rect();
        }

        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;
        Vector2 cameraCenter = camera.transform.position;
        return new Rect(cameraCenter.x - cameraWidth * 0.5f, cameraCenter.y - cameraHeight * 0.5f, cameraWidth, cameraHeight);
    }


    // Interface Methods
    public void TakeDamage(float amount)
    {
        damageAct.amount = amount;
        damageAct.Perform();
    }
    public void Trap(float duration)
    {
        moveAct.SetEnabled(false);
    }
    public void Untrap()
    {
        moveAct.SetEnabled(true);
    }


    // Override Methods
    void Update()
    {
        // Move
        moveAct.border = GetBorderFromCamera(Camera.main);
        moveAct.direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        moveAct.Perform();


        // Look towards mouse constantly
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookAct.targetRotation = lookAct.RotationTowardsPosition(mouseWorldPosition);


        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 spawnOrigin = barrelObject?.transform.position ?? transform.position;
            shootAct.direction = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
            shootAct.spawnLocation = spawnOrigin;
            shootAct.Perform();
        }
    }
    void Awake()
    {
        // Setup acts
        theater = GetComponent<Theater>();

        lookAct.turnType = LookAct.TurnType.Continuous;
        lookAct.turnSpeed = -1.0f;
        lookAct.Init(theater, "Look Act");
        lookAct.Perform();

        moveAct.border = GetBorderFromCamera(Camera.main);
        moveAct.useBorder = true;
        moveAct.Init(theater, "Move Act");

        shootAct.spawnAtOwner = false;
        shootAct.Init(theater, "Shoot Act");

        damageAct.toFlash = true;
        damageAct.healthSystem = healthSystem;
        damageAct.OnPostExit += (Act act) =>
        {
            if (Mathf.Approximately(healthSystem.currentHealth, 0.0f))
            {
                OnKilled?.Invoke();
            }
        };
        damageAct.Init(theater, "Damage Act");
    }
}
