using UnityEngine;

namespace com.game
{
    public class ParryShield : MonoBehaviour
    {
       [Header("Reflection Settings")]
       [SerializeField] private float reflectAngleOffset = 0f; // Adjustable reflection angle
       [SerializeField] private float reflectSpeedMultiplier = 1.5f; // Speed multiplier after reflection
        
       private void OnTriggerEnter(Collider other)
       {
           if (other.gameObject.TryGetComponent(out EnemyProjectile projectile))
               ReflectProjectileTrigger(projectile);
       }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out EnemyProjectile projectile))
                ReflectProjectile(projectile, collision.contacts[0].normal);
        }

        private void ReflectProjectile(EnemyProjectile projectile, Vector3 contactPoint)
        {
            // Gelen merminin hýz yönünü al
            Vector3 incomingDirection = projectile.GetVelocity().normalized;

            float currProjectileSpeed = projectile.GetVelocity().magnitude;

            // Gelen mermiyi çarpýþma normaline göre yansýt
            Vector3 reflectDirection = Vector3.Reflect(incomingDirection, contactPoint);

            // Hýz çarpaný uygula
            projectile.ApplyVelocity(reflectDirection * Mathf.Max(currProjectileSpeed * reflectSpeedMultiplier, 0));

            Debug.Log("Projectile Reflected!");
        }
        private void ReflectProjectileTrigger(EnemyProjectile projectile)
        {
            // Gelen merminin hýz yönünü al
            Vector3 incomingDirection = projectile.GetVelocity().normalized;

            // Çarpýþma noktasýndaki normal vektörünü al
            Vector3 contactNormal = (projectile.GetRigidbody().position - transform.position).normalized;

            // Gelen mermiyi çarpýþma normaline göre yansýt
            Vector3 reflectDirection = Vector3.Reflect(incomingDirection, contactNormal);

            // Hýz çarpaný uygula
            projectile.ApplyVelocity(reflectDirection * projectile.GetVelocity().magnitude * reflectSpeedMultiplier);

            Debug.Log("Projectile Reflected!");
        }
    }
}
