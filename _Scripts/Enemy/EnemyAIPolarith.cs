using com.absence.attributes;
using com.absence.attributes.experimental;
using Polarith.AI.Move;
using UnityEngine;

namespace com.game.enemysystem.ai
{
    public class EnemyAIPolarith : MonoBehaviour, IEnemyAI
    {
        [Header1("Enemy AI - Polarith")]

        [Header2("Runtime")]

        [SerializeField, Readonly] private EnemyAIState m_state = EnemyAIState.Chasing;

        [Space, Header2("Fields")]

        [SerializeField, Required] private Rigidbody m_rigidbody;
        [SerializeField, Required] private AIMContext m_chasingContext;
        [SerializeField, Required, InlineEditor] private EnemyMovementDataPolarith m_movementData;

        [Space, Header2("Settings")]
        [SerializeField] private float m_rotationMinDeadZone = 0.01f;

        public EnemyAIState State
        {
            get => m_state;
            set => m_state = value;
        }
        public bool Enabled
        {
            get => m_enabled;
            set => m_enabled = value;
        }
        public bool Locked
        {
            get => m_isLocked;
            set => m_isLocked = value;
        }
        public float Speed
        {
            get => m_speed;
            set => m_speed = value;
        }

        public EnemyMovementData MovementData => m_movementData;
        public float DefaultSpeed => m_defaultSpeed;

        Transform m_target;
        float m_defaultSpeed;
        float m_speed;
        bool m_enabled;
        bool m_isLocked;

        public void Initialize(Transform target)
        {
            Refresh(target);
        }

        public void Refresh(Transform target)
        {
            m_target = target;
            if (m_target == null)
            {
                Debug.LogWarning("Target of a Polarith AI agent is null!");
                return;
            }

            m_defaultSpeed = m_movementData.speed;

            HandleState();
            HandleRotation();
            HandleMovement();
        }

        void HandleState()
        {
            if (m_state == EnemyAIState.Dead)
                return;

            float distanceToTarget = Vector3.Distance(transform.position, m_target.position);

            if (distanceToTarget <= m_movementData.stoppingDistance)
                m_state = EnemyAIState.Attacking;
            else
                m_state = EnemyAIState.Chasing;
        }

        void HandleMovement()
        {
            if (m_state != EnemyAIState.Chasing)
                return;

            Vector3 delta = m_speed * Time.deltaTime * m_chasingContext.DecidedDirection;

            m_rigidbody.linearVelocity = delta;
        }

        void HandleRotation()
        {
            //Quaternion newRotation = m_kinematicRigidbody.rotation;

            //float angleDifference = 
            //    Vector3.SignedAngle(transform.forward, m_chasingContext.DecidedDirection, Vector3.up);

            //float differenceMultiplier = 0f;

            //if (angleDifference < -m_rotationMinDeadZone)
            //    differenceMultiplier = -1f;
            //else if (angleDifference > m_rotationMinDeadZone)
            //    differenceMultiplier = 1f;

            //newRotation *= Quaternion.Euler(Vector3.up * differenceMultiplier * 
            //    m_movementData.angularSpeed * Time.deltaTime);

            //m_kinematicRigidbody.MoveRotation(newRotation);

            float t = Time.deltaTime * m_movementData.angularSpeed;
            Vector3 newRotationEulers = Vector3.Lerp(transform.forward, m_chasingContext.DecidedDirection, t);
            transform.forward = newRotationEulers;
        }
    }
}
