using com.absence.attributes;
using com.absence.attributes.experimental;
using UnityEngine;
using UnityEngine.AI;

namespace com.game.enemysystem.ai
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAINavMeshAgent : MonoBehaviour, IEnemyAI
    {
        public const int MAX_AGENTS_CAN_BE_DETECTED = 8;

        [SerializeField, Readonly] private NavMeshAgent m_navMeshAgent;
        [SerializeField, Readonly] private EnemyAIState m_state;
        [SerializeField] private Collider m_selfCollider;
        [SerializeField] private LayerMask m_agentDetectionMask;
        [SerializeField] private float m_agentDetectionRadius;
        [SerializeField] private float m_fleeStrength;
        [SerializeField, InlineEditor] private EnemyMovementDataNavMeshAgent m_movementData;

        public EnemyAIState State
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
                Refresh(m_target);
            }
        }
        public EnemyMovementData MovementData
        {
            get
            {
                return m_movementData;
            }
        }
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }

            set
            {
                m_enabled = value;
                m_navMeshAgent.enabled = value;
            }
        }
        public bool Locked
        {
            get => m_isLocked;
            set => m_isLocked = value;
        }
        public float Speed
        {
            get => m_navMeshAgent.speed;
            set => m_navMeshAgent.speed = value;
        }
        public float DefaultSpeed => m_defaultSpeed;

        float m_defaultSpeed;
        bool m_initialized;
        bool m_enabled = true;
        Transform m_target;
        bool m_isLocked;
        Collider[] m_agentsFound;

        private void Start()
        {
            m_agentsFound = new Collider[MAX_AGENTS_CAN_BE_DETECTED];
        }

        private void Update()
        {
            if (!m_initialized)
                return;

            ApplyPrerequisites();

            if (!Enabled)
                return;

            if (!m_navMeshAgent.hasPath) 
                return;

            RotateTowardsTarget();
        }

        void ApplyPrerequisites()
        {
            m_navMeshAgent.isStopped = Locked;
        }

        public void Initialize(Transform target)
        {
            m_defaultSpeed = m_movementData.speed;
            m_navMeshAgent.updateRotation = false;

            Speed = m_movementData.speed;
            m_navMeshAgent.angularSpeed = m_movementData.angularSpeed;
            m_navMeshAgent.acceleration = m_movementData.acceleration;
            m_navMeshAgent.stoppingDistance = m_movementData.stoppingDistance;

            Refresh(target);

            m_initialized = true;
        }

        public void Refresh(Transform target)
        {
            m_target = target;

            m_navMeshAgent.SetDestination(m_target.position);

            //Vector3 diff = m_target.position - transform.position;
            //Vector3 direction = diff.normalized;
            //float distance = diff.sqrMagnitude;

            //int count = 
            //    Physics.OverlapSphereNonAlloc(transform.position, m_agentDetectionRadius, m_agentsFound, m_agentDetectionMask);

            //foreach (Collider collider in m_agentsFound)
            //{
            //    if (collider == null)
            //        continue;

            //    if (m_selfCollider != null && collider == m_selfCollider)
            //        continue;

            //    direction += 
            //        ((transform.position - collider.transform.position).normalized * m_fleeStrength) / count;
            //}

            //m_navMeshAgent.SetDestination(transform.position + (direction * distance));
        }

        void RotateTowardsTarget()
        {
            Vector3 direction = (m_navMeshAgent.steeringTarget - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_navMeshAgent.angularSpeed);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, m_agentDetectionRadius);
        }

        private void Reset()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}
