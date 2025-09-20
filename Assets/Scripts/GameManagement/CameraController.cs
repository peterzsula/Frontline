using UnityEngine;

namespace Frontline.GameManagement
{
    /// <summary>
    /// Third-person camera controller for following the player tank
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private float rotationSpeed = 1f;
        
        [Header("Mouse Look")]
        [SerializeField] private bool enableMouseLook = true;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -30f;
        [SerializeField] private float maxVerticalAngle = 60f;
        
        private Vector3 currentVelocity;
        private float mouseX;
        private float mouseY;
        
        private void Start()
        {
            // Find player tank if not assigned
            if (target == null)
            {
                GameObject playerTank = GameObject.FindGameObjectWithTag("Player");
                if (playerTank != null)
                {
                    target = playerTank.transform;
                }
            }
            
            // Lock cursor for mouse look
            if (enableMouseLook)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        private void Update()
        {
            if (enableMouseLook)
            {
                HandleMouseLook();
            }
            
            // Toggle cursor lock with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            FollowTarget();
        }
        
        private void HandleMouseLook()
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);
        }
        
        private void FollowTarget()
        {
            // Calculate desired position
            Vector3 desiredPosition;
            
            if (enableMouseLook)
            {
                // Apply mouse rotation to offset
                Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
                desiredPosition = target.position + rotation * offset;
            }
            else
            {
                // Follow behind the target
                desiredPosition = target.position + target.TransformDirection(offset);
            }
            
            // Smoothly move to desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / followSpeed);
            
            // Look at target
            Vector3 lookDirection = target.position - transform.position;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
    }
}