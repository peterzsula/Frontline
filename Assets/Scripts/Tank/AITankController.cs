using UnityEngine;

namespace Frontline.Tank
{
    /// <summary>
    /// Basic AI controller for enemy tanks - prototype implementation
    /// </summary>
    public class AITankController : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float detectionRange = 20f;
        [SerializeField] private float attackRange = 15f;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private LayerMask enemyLayers = 1; // Default layer only
        
        private TankController tankController;
        private TankShooting tankShooting;
        private TankHealth tankHealth;
        
        private Transform target;
        private Vector3 wanderTarget;
        private float nextWanderTime;
        private float nextFireTime;
        
        private enum AIState
        {
            Wandering,
            Chasing,
            Attacking
        }
        
        private AIState currentState = AIState.Wandering;
        
        private void Awake()
        {
            tankController = GetComponent<TankController>();
            tankShooting = GetComponent<TankShooting>();
            tankHealth = GetComponent<TankHealth>();
        }
        
        private void Start()
        {
            SetNewWanderTarget();
        }
        
        private void Update()
        {
            if (tankHealth != null && tankHealth.IsDead)
                return;
                
            FindTarget();
            UpdateAI();
        }
        
        private void FindTarget()
        {
            Collider[] potentialTargets = Physics.OverlapSphere(transform.position, detectionRange, enemyLayers);
            
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;
            
            foreach (var collider in potentialTargets)
            {
                if (collider.transform == transform) continue;
                
                // Check if it's an enemy tank (for prototype, target player tanks)
                if (collider.CompareTag("Player"))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = collider.transform;
                    }
                }
            }
            
            target = closestTarget;
        }
        
        private void UpdateAI()
        {
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                
                if (distanceToTarget <= attackRange)
                {
                    currentState = AIState.Attacking;
                    AttackTarget();
                }
                else if (distanceToTarget <= detectionRange)
                {
                    currentState = AIState.Chasing;
                    ChaseTarget();
                }
                else
                {
                    target = null;
                    currentState = AIState.Wandering;
                }
            }
            else
            {
                currentState = AIState.Wandering;
                Wander();
            }
        }
        
        private void AttackTarget()
        {
            // Face the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
            
            // Fire at the target
            if (Time.time >= nextFireTime && tankShooting != null)
            {
                if (tankShooting.Fire())
                {
                    nextFireTime = Time.time + Random.Range(1f, 3f); // Random fire interval
                }
            }
            
            // Move slightly to avoid being a stationary target
            Vector3 perpendicular = Vector3.Cross(directionToTarget, Vector3.up).normalized;
            Vector3 strafeDirection = perpendicular * Mathf.Sin(Time.time * 0.5f);
            
            MoveInDirection(strafeDirection * 0.3f);
        }
        
        private void ChaseTarget()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            MoveInDirection(directionToTarget);
        }
        
        private void Wander()
        {
            if (Time.time >= nextWanderTime)
            {
                SetNewWanderTarget();
            }
            
            Vector3 directionToWanderTarget = (wanderTarget - transform.position).normalized;
            MoveInDirection(directionToWanderTarget);
            
            // If close to wander target, set a new one
            if (Vector3.Distance(transform.position, wanderTarget) < 2f)
            {
                SetNewWanderTarget();
            }
        }
        
        private void SetNewWanderTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            wanderTarget = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            nextWanderTime = Time.time + Random.Range(3f, 8f);
        }
        
        private void MoveInDirection(Vector3 direction)
        {
            if (tankController == null)
            {
                Debug.LogWarning("AITankController: TankController component missing! AI cannot move properly.");
                return;
            }
            
            // Calculate movement input
            float moveInput = Vector3.Dot(transform.forward, direction);
            float rotateInput = Vector3.Cross(transform.forward, direction).y;
            
            // Apply input to tank controller (simplified AI input)
            // Since we can't directly access the private input handling, we'll simulate it
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Move forward/backward
                if (Mathf.Abs(moveInput) > 0.1f)
                {
                    Vector3 movement = transform.forward * moveInput * 8f * Time.deltaTime;
                    rb.AddForce(movement, ForceMode.VelocityChange);
                }
                
                // Rotate
                if (Mathf.Abs(rotateInput) > 0.1f)
                {
                    float rotation = rotateInput * 60f * Time.deltaTime;
                    rb.AddTorque(0, rotation, 0, ForceMode.VelocityChange);
                }
            }
            else
            {
                Debug.LogWarning("AITankController: Rigidbody component missing! AI cannot move.");
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Draw wander target
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wanderTarget, 0.5f);
        }
    }
}