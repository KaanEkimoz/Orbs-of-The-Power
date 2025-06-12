using com.absence.attributes;
using com.game.generics.interfaces;
using UnityEngine;

namespace com.game.generics
{
    [RequireComponent(typeof(BoxCollider))]
    public class DeathZone : MonoBehaviour
    {
        [SerializeField, Readonly] private BoxCollider m_collider;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(m_collider.center, m_collider.size);
        }

        private void Reset()
        {
            m_collider = GetComponent<BoxCollider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null) Apply(collision.rigidbody.gameObject);
            else Apply(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null) Apply(other.attachedRigidbody.gameObject);
            else Apply(other.gameObject);
        }

        void Apply(GameObject other)
        {
            if (other.TryGetComponent(out IDamageable damageable)) damageable.Die(DeathCause.Internal);
            else if (other.TryGetComponent(out ICustomDestroy customDestroy)) customDestroy.CustomDestroy();
            else Destroy(other);
        }
    }
}
