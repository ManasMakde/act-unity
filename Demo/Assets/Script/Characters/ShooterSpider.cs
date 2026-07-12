using UnityEngine;


public class ShooterSpider : SpiderBase
{
    // Act Properties
    [SerializeField] PerpetualAct liveAct = new();
    [SerializeField] GotoAct wanderAct = new();
    [SerializeField] WaitAct waitAct = new();
    [SerializeField] LookAct lookAct = new();
    [SerializeField] LookAct aimAct = new();
    [SerializeField] ShootAct shootAct = new();


    // Static Methods
    public static Vector3[] GetCameraCornersOnGround(Camera camera)  // Ground= x-y plane
    {
        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[]
        {
            new Vector3(0, 0, camera.nearClipPlane),
            new Vector3(0, 1, camera.nearClipPlane),
            new Vector3(1, 1, camera.nearClipPlane),
            new Vector3(1, 0, camera.nearClipPlane)
        };

        float groundZ = 0.0f;  // z where sprites live
        Plane targetPlane = new Plane(Vector3.forward, new Vector3(0, 0, groundZ));
        for (int i = 0; i < 4; i++)
        {
            Ray ray = camera.ViewportPointToRay(screenCorners[i]);
            if (targetPlane.Raycast(ray, out float enterDistance))
            {
                corners[i] = ray.GetPoint(enterDistance);
            }
            else
            {
                Debug.LogWarning("GetCameraCornersOnGround ray missed ground plane, using zero corner");
                corners[i] = Vector3.zero;
            }
        }

        return corners;
    }
    public static Vector3 GetRandomPointInView(Camera camera)
    {
        Vector3[] corners = GetCameraCornersOnGround(camera);
        Vector3 p0 = corners[0];
        Vector3 p1 = corners[1];
        Vector3 p2 = corners[2];
        Vector3 p3 = corners[3];

        float areaA = Vector3.Cross(p1 - p0, p2 - p0).magnitude * 0.5f;
        float areaB = Vector3.Cross(p2 - p0, p3 - p0).magnitude * 0.5f;
        float totalArea = areaA + areaB;

        Vector3 a, b, c;
        if (Random.value < (areaA / totalArea))
        {
            a = p0; b = p1; c = p2;
        }
        else
        {
            a = p0; b = p2; c = p3;
        }

        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        float r3 = 1f - r1 - r2;
        return (r1 * a) + (r2 * b) + (r3 * c);
    }


    // Private Methods
    private Vector2 GetLookDirection()  // current facing direction based on aimAct rotation
    {
        float rotationRad = aimAct.rb.rotation * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rotationRad), Mathf.Sin(rotationRad));
    }


    // Override Properties
    protected override void Awake()
    {
        // Animate when damaged
        damageAct.toAnimate = true;
        damageAct.AddToBlock(new() { liveAct });  // Stop AI behaviour while damage animation is being played


        base.Awake();


        // Setup Live Act
        liveAct.Prologue += (Act act) =>
        {
            // Do not shoot if player is already stuck
            bool isPlayerStuck = playerTransform == null || (playerTransform.GetComponentInChildren<StickyEffect>() != null);


            // Set random location to wander & look towards
            Vector2 randomPosition = GetRandomPointInView(Camera.main);
            wanderAct.location = randomPosition;
            lookAct.targetRotation = lookAct.RotationTowardsPosition(randomPosition);


            // Wander to Random positions -> Aim -> Shoot -> Wait
            return Act.Seq(new() {
                new() { wanderAct, lookAct },
                isPlayerStuck? new() {} : new() { aimAct },  // Don't aim if player is already stuck
                isPlayerStuck ? new() {} : new() { shootAct },  // Don't shoot if player is already stuck
                new() { waitAct }
            });
        };
        liveAct.Init(theater, "Live Act");


        // Wander Act
        wanderAct.Init(theater, "Wander Act");


        // Wait Act
        waitAct.Init(theater, "Wander Wait Act");


        // Look Act
        lookAct.turnType = LookAct.TurnType.UntilFacing;
        lookAct.Init(theater, "look Act");


        // Aim Act
        aimAct.targetTransform = playerTransform;
        aimAct.turnType = LookAct.TurnType.Continuous;
        aimAct.followTimeout = 2.0f;
        aimAct.Init(theater, "Aim Act");


        // Shoot Act
        shootAct.OnPreEnter += (Act act) =>
        {
            shootAct.direction = GetLookDirection();
        };
        shootAct.Init(theater, "Shoot Act");
    }
}
