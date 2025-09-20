using UnityEngine;
using UnityEngine.Events;

namespace Frontline.Tank
{
    /// <summary>
    /// Manages tank health, damage, and destruction
    /// </summary>
    public class TankHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        [Header("Events")]
        public UnityEvent<float> OnHealthChanged;
        public UnityEvent OnTankDestroyed;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private AudioClip explosionSound;
        
        private AudioSource audioSource;
        private bool isDead = false;
        
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float HealthPercentage => currentHealth / maxHealth;
        public bool IsDead => isDead;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            OnHealthChanged?.Invoke(currentHealth);
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(float healAmount)
        {
            if (isDead) return;
            
            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        private void Die()
        {
            if (isDead) return;
            
            isDead = true;
            
            // Create explosion effect
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
            }
            
            // Play explosion sound
            if (audioSource != null && explosionSound != null)
            {
                audioSource.PlayOneShot(explosionSound);
            }
            
            OnTankDestroyed?.Invoke();
            
            // Disable tank components
            var tankController = GetComponent<TankController>();
            if (tankController != null)
                tankController.enabled = false;
                
            var tankShooting = GetComponent<TankShooting>();
            if (tankShooting != null)
                tankShooting.enabled = false;
            
            // Optional: Destroy after delay to allow explosion effects
            Destroy(gameObject, 3f);
        }
        
        public void Reset()
        {
            currentHealth = maxHealth;
            isDead = false;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}