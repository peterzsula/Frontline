using UnityEngine;
using UnityEngine.InputSystem;

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
        private Vector2 moveInput;
        private bool fireInput;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            tankShooting = GetComponent<TankShooting>();
            tankHealth = GetComponent<TankHealth>();
        }
        
        private void Update()
        {
            // Handle input - using new Input System
            HandleInput();
            
            if (fireInput && tankShooting != null)
            {
                tankShooting.Fire();
                fireInput = false; // Reset fire input
            }
        }
        
        private void FixedUpdate()
        {
            Move();
            Rotate();
        }
        
        private void HandleInput()
        {
            // Get movement input from WASD or arrow keys
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                Vector2 input = Vector2.zero;
                
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                    input.y = 1f;
                else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                    input.y = -1f;
                    
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    input.x = 1f;
                else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    input.x = -1f;
                    
                moveInput = input;
                
                // Fire input
                if (keyboard.spaceKey.wasPressedThisFrame)
                    fireInput = true;
            }
            
            // Also check for mouse input for firing
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                fireInput = true;
            }
        }
        
        private void Move()
        {
            Vector3 movement = transform.forward * moveInput.y * moveSpeed * Time.fixedDeltaTime;
            
            // Apply movement while respecting max speed
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(movement, ForceMode.VelocityChange);
            }
        }
        
        private void Rotate()
        {
            float rotation = moveInput.x * rotationSpeed * Time.fixedDeltaTime;
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