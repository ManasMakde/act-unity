// using UnityEngine;

// public class ShooterSpider : SpiderBase
// {
//     private enum ShooterState
//     {
//         Kiting,
//         Charging,
//         Dead
//     }


//     // Private Properties
//     [SerializeField] private float preferredDistance = 5f;
//     [SerializeField] private float shootInterval = 2f;
//     [SerializeField] private float chargeShootInterval = 0.5f;
//     [SerializeField] private float circleStrafeSpeed = 90f;
//     [SerializeField] private float lowHealthFraction = 0.3f;
//     [SerializeField] private float chargeCheckInterval = 1f;
//     [SerializeField] private float chargeChance = 0.3f;
//     [SerializeField] private GameObject webProjectilePrefab;
//     [SerializeField] private float fullCircleDegrees = 360f;
//     private ShooterState currentState = ShooterState.Kiting;
//     private float shootTimer;
//     private float chargeCheckTimer;
//     private float angleTraveled;
//     private Vector3 chargeCenter;


//     // Public Methods
//     public override void Attack()
//     {
//         if (webProjectilePrefab == null)
//         {
//             Debug.LogWarning("Attack called but web projectile prefab missing");
//             return;
//         }

//         if (playerTransform == null)
//         {
//             Debug.LogWarning("Attack called but player transform missing");
//             return;
//         }

//         Vector2 direction = (playerTransform.position - transform.position).normalized;
//         GameObject webObj = Instantiate(webProjectilePrefab, transform.position, Quaternion.identity);
//         Web web = webObj.GetComponent<Web>();
//         if (web == null)
//         {
//             Debug.LogWarning("Attack instantiated web with no Web component");
//             return;
//         }

//         web.SetDirection(direction);
//     }


//     // Private Methods
//     private void UpdateKiting()
//     {
//         if (playerTransform == null)
//         {
//             Debug.LogWarning("UpdateKiting called but player transform missing");
//             return;
//         }

//         float distance = GetDistanceToPlayer();
//         if (distance > detectionRange)
//         {
//             return;
//         }

//         if (distance > preferredDistance)
//         {
//             MoveTowardsPlayer();
//         }
//         else if (distance < preferredDistance)
//         {
//             MoveAwayFromPlayer();
//         }

//         shootTimer -= Time.deltaTime;
//         if (shootTimer <= 0f)
//         {
//             Attack();
//             shootTimer = shootInterval;
//         }

//         chargeCheckTimer -= Time.deltaTime;
//         if (chargeCheckTimer <= 0f)
//         {
//             TryStartCharge();
//             chargeCheckTimer = chargeCheckInterval;
//         }
//     }
//     private void TryStartCharge()
//     {
//         float healthFraction = healthSystem.CurrentHealth / healthSystem.MaxHealth;
//         if (healthFraction > lowHealthFraction)
//         {
//             return;
//         }

//         float roll = Random.value;
//         if (roll > chargeChance)
//         {
//             return;
//         }

//         StartCharge();
//     }
//     private void StartCharge()
//     {
//         if (playerTransform == null)
//         {
//             Debug.LogWarning("StartCharge called but player transform missing");
//             return;
//         }

//         currentState = ShooterState.Charging;
//         chargeCenter = playerTransform.position;
//         angleTraveled = 0f;
//         shootTimer = chargeShootInterval;
//     }
//     private void UpdateCharging()
//     {
//         Vector3 offset = transform.position - chargeCenter;
//         float stepAngle = circleStrafeSpeed * Time.deltaTime;

//         offset = Quaternion.Euler(0f, 0f, stepAngle) * offset;
//         transform.position = chargeCenter + offset;
//         angleTraveled += Mathf.Abs(stepAngle);

//         shootTimer -= Time.deltaTime;
//         if (shootTimer <= 0f)
//         {
//             Attack();
//             shootTimer = chargeShootInterval;
//         }

//         if (angleTraveled >= fullCircleDegrees)
//         {
//             currentState = ShooterState.Dead;
//             Destroy(gameObject);
//         }
//     }
//     private void MoveAwayFromPlayer()
//     {
//         if (playerTransform == null)
//         {
//             Debug.LogWarning("MoveAwayFromPlayer called but player transform missing");
//             return;
//         }

//         Vector2 direction = (transform.position - playerTransform.position).normalized;
//         transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
//     }


//     // Override Methods
//     private void Update()
//     {
//         if (currentState == ShooterState.Kiting)
//         {
//             UpdateKiting();
//             return;
//         }

//         if (currentState == ShooterState.Charging)
//         {
//             UpdateCharging();
//         }
//     }
// }
