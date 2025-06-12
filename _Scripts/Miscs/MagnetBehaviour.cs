using com.absence.attributes;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.miscs
{
    public class MagnetBehaviour : MonoBehaviour
    {
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
        }

        public enum MagnetType
        {
            [InspectorName("Physics (Force)")] Physics,
            [InspectorName("Virtual (DOTween)")] Virtual,
            [InspectorName("Transform (Translate)")] Transform,
            [InspectorName("Transform (Lerp)")] Lerp,
        }

        [SerializeField] private MagnetType m_magnetType = MagnetType.Physics;
        [SerializeField] private UpdateMode m_updateMode = UpdateMode.FixedUpdate;
        [SerializeField, ShowIf(nameof(m_magnetType), MagnetType.Physics)] private LayerMask m_layerMask;
        [SerializeField, ShowIf(nameof(m_magnetType), MagnetType.Physics)] private ForceMode m_forceMode;
        [SerializeField, ShowIf(nameof(m_magnetType), MagnetType.Virtual)] private Ease m_ease;
        [SerializeField] private bool m_useDeltaTime;
        [SerializeField, ShowIf(nameof(m_magnetType), MagnetType.Lerp)] private float m_smoothing;
        [SerializeField] private float m_minRadius;
        [SerializeField] private float m_maxAffectionRadius;
        [SerializeField] private float m_strength;

        float p_deltaTime 
        { 
            get
            {
                return m_useDeltaTime ? Time.deltaTime : 1f;
            }
        }

        Collider[] m_found;
        Dictionary<Rigidbody, Tweener> m_followers = new();

        private void FixedUpdate()
        {
            m_found = Physics.OverlapSphere(transform.position, m_maxAffectionRadius, m_layerMask);

            if (m_updateMode == UpdateMode.FixedUpdate)
                ApplyMagnet();
        }

        private void Update()
        {
            if (m_updateMode == UpdateMode.Update)
                ApplyMagnet();
        }

        void ApplyMagnet()
        {
            foreach (Collider collider in m_found)
            {
                if (!collider.attachedRigidbody.TryGetComponent(out IMagnetable magnetable))
                    continue;

                if (!magnetable.IsMagnetable)
                    continue;

                switch (m_magnetType)
                {
                    case MagnetType.Physics:
                        ApplyPhysicsMagnet(collider.attachedRigidbody, magnetable);
                        break;
                    case MagnetType.Virtual:
                        ApplyVirtualMagnet(collider.attachedRigidbody, magnetable);
                        break;
                    case MagnetType.Transform:
                        ApplyTransformMagnet(collider.attachedRigidbody.transform, magnetable);
                        break;
                    case MagnetType.Lerp:
                        ApplyTransformMagnet(collider.attachedRigidbody.transform, magnetable, true);
                        break;
                    default:
                        break;
                }
            }

            foreach (var follower in m_followers)
            {
                follower.Value.ChangeEndValue(transform.position, false);
            }
        }

        void ApplyVirtualMagnet(Rigidbody rigidbody, IMagnetable magnetable)
        {
            if (m_followers.ContainsKey(rigidbody))
                return;

            Tweener tweener = rigidbody.DOMove(transform.position, 1 / (m_strength + 0.00001f))
                .SetEase(m_ease)
                .OnComplete(() => m_followers.Remove(rigidbody))
                .OnKill(() => m_followers.Remove(rigidbody));

            m_followers.Add(rigidbody, tweener);
        }

        void ApplyPhysicsMagnet(Rigidbody rigidbody, IMagnetable magnetable)
        {
            Vector3 rawDirection = transform.position - rigidbody.transform.position;
            //rawDirection.y = 0f;
            //Vector3 negGravityForce = (-Physics.gravity) * rigidbody.mass;
            Vector3 forceDirection = (rawDirection).normalized;

            float multiplier = 1 / 
                Mathf.Pow(GetDistance(rigidbody.transform.position) + magnetable.MagnetResistance, 2);

            rigidbody.AddForce(forceDirection * m_strength * multiplier * p_deltaTime, m_forceMode);
        }

        void ApplyTransformMagnet(Transform target, IMagnetable magnetable, bool lerp = false)
        {
            Vector3 rawDirection = transform.position - target.position;
            //rawDirection.y = 0f;
            //Vector3 negGravityForce = (-Physics.gravity) * rigidbody.mass;
            Vector3 moveDirection = rawDirection.normalized;

            float multiplier = 1 /
                Mathf.Pow(GetDistance(target.position) + magnetable.MagnetResistance, 2);

            Vector3 delta = multiplier * moveDirection * m_strength;

            if (lerp) target.position = Vector3.Lerp(target.position, target.position + delta, p_deltaTime * m_smoothing);
            else target.Translate(delta * p_deltaTime, Space.World);
        }

        float GetDistance(Vector3 position)
        {
            return Mathf.Max(m_minRadius, Vector3.Distance(transform.position, position));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, m_minRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_maxAffectionRadius);
        }
    }
}
