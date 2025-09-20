using UnityEngine;

namespace Frontline.Tank
{
    /// <summary>
    /// Main controller for tank movement, rotation, and basic input handling
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TankController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float maxSpeed = 15f;
        
        [Header("Tank Components")]
        [SerializeField] private Transform turret;
        [SerializeField] private Transform firePoint;
        
        private Rigidbody rb;
        private TankShooting tankShooting;
        private TankHealth tankHealth;
        
        // Input values
        private float moveInput;
        private float rotateInput;
        private bool fireInput;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            tankShooting = GetComponent<TankShooting>();
            tankHealth = GetComponent<TankHealth>();
        }
        
        private void Update()
        {
            // Handle input - this would be replaced with proper input system in full game
            HandleInput();
            
            if (fireInput && tankShooting != null)
            {
                tankShooting.Fire();
            }
        }
        
        private void FixedUpdate()
        {
            Move();
            Rotate();
        }
        
        private void HandleInput()
        {
            moveInput = Input.GetAxis("Vertical");
            rotateInput = Input.GetAxis("Horizontal");
            fireInput = Input.GetButtonDown("Fire1");
        }
        
        private void Move()
        {
            Vector3 movement = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
            
            // Apply movement while respecting max speed
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(movement, ForceMode.VelocityChange);
            }
        }
        
        private void Rotate()
        {
            float rotation = rotateInput * rotationSpeed * Time.fixedDeltaTime;
            rb.AddTorque(0, rotation, 0, ForceMode.VelocityChange);
        }
        
        public void SetTurret(Transform newTurret)
        {
            turret = newTurret;
        }
        
        public void SetFirePoint(Transform newFirePoint)
        {
            firePoint = newFirePoint;
        }
    }
}