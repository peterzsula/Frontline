using UnityEngine;

namespace Frontline.Tank
{
    /// <summary>
    /// Handles tank shooting mechanics including projectile firing and ammo management
    /// </summary>
    public class TankShooting : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireForce = 1000f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private int maxAmmo = 10;
        [SerializeField] private float reloadTime = 3f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private AudioClip reloadSound;
        
        private AudioSource audioSource;
        private int currentAmmo;
        private float nextFireTime = 0f;
        private bool isReloading = false;
        
        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public bool IsReloading => isReloading;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            currentAmmo = maxAmmo;
        }
        
        public bool Fire()
        {
            if (Time.time < nextFireTime || currentAmmo <= 0 || isReloading)
                return false;
                
            // Create projectile
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
                
                if (projectileRb != null)
                {
                    projectileRb.AddForce(firePoint.forward * fireForce);
                }
            }
            
            // Update ammo and fire rate
            currentAmmo--;
            nextFireTime = Time.time + fireRate;
            
            // Play sound
            if (audioSource != null && fireSound != null)
            {
                audioSource.PlayOneShot(fireSound);
            }
            
            // Auto-reload if out of ammo
            if (currentAmmo <= 0)
            {
                StartReload();
            }
            
            return true;
        }
        
        public void StartReload()
        {
            if (isReloading || currentAmmo >= maxAmmo)
                return;
                
            StartCoroutine(ReloadCoroutine());
        }
        
        private System.Collections.IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            
            // Play reload sound
            if (audioSource != null && reloadSound != null)
            {
                audioSource.PlayOneShot(reloadSound);
            }
            
            yield return new WaitForSeconds(reloadTime);
            
            currentAmmo = maxAmmo;
            isReloading = false;
        }
        
        public void SetFirePoint(Transform newFirePoint)
        {
            firePoint = newFirePoint;
        }
    }
}