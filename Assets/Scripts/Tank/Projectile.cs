using UnityEngine;

namespace Frontline.Tank
{
    /// <summary>
    /// Projectile behavior for tank shells including damage and physics
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float damage = 25f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private LayerMask damageableLayers = -1;
        
        [Header("Effects")]
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private AudioClip hitSound;
        
        private Rigidbody rb;
        private bool hasHit = false;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            // Destroy projectile after lifetime
            Destroy(gameObject, lifetime);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (hasHit) return;
            
            hasHit = true;
            
            // Check if we hit something damageable
            if (IsInLayerMask(collision.gameObject.layer, damageableLayers))
            {
                // Try to damage the target
                TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
                if (tankHealth != null)
                {
                    tankHealth.TakeDamage(damage);
                }
            }
            
            // Create hit effect
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Play hit sound
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        
        private bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }
        
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }
        
        public void SetLifetime(float newLifetime)
        {
            lifetime = newLifetime;
        }
    }
}