using DG.Tweening;
using UnityEngine;

namespace com.game.generics
{
    public class RotatePingPong : MonoBehaviour
    {
        public enum Axis
        {
            X,
            Y,
            Z,
        }

        [SerializeField] private float m_gizmoRange = 1;
        [SerializeField] private bool m_startOnAwake;
        [SerializeField] private Transform m_target;
        [SerializeField] private Axis m_targetAxis;
        [SerializeField] private Ease m_ease;
        [SerializeField] private float m_startingAngleShift;
        [SerializeField] private float m_angle;
        [SerializeField] private float m_duration;
        [SerializeField] private float m_delay;
        [SerializeField] private int m_loops = -1;

        Transform p_transform => m_target != null ? m_target : transform;

        Vector3 m_defaultEulerAngles;
        Tween m_tween;
        Vector3 m_axis;

        private void Awake()
        {
            m_axis = GetAxis();

            p_transform.localEulerAngles = 
                GetRotatedVector(p_transform.localEulerAngles, -m_startingAngleShift);

            if (m_startOnAwake)
                Begin();
        }

        public void Begin()
        {
            m_defaultEulerAngles = p_transform.localEulerAngles;

            m_tween = p_transform.DOLocalRotate(GetRotatedVector(m_defaultEulerAngles, m_angle), m_duration)
                .SetDelay(m_delay)
                .SetEase(m_ease)
                .SetLoops(m_loops, LoopType.Yoyo);
        }

        public Vector3 GetRotatedVector(Vector3 target, float angle)
        {
            return GetRotatedVector(target, angle, m_axis);
        }

        public Vector3 GetRotatedVector(Vector3 originalEuler, float angle, Vector3 axis)
        {
            return (Quaternion.AngleAxis(angle, axis) * Quaternion.Euler(originalEuler)).eulerAngles;
        }

        public Vector3 GetAxis()
        {
            return m_targetAxis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.zero,
            };
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 axis = GetAxis();

            Quaternion startRotation = Quaternion.AngleAxis(-m_startingAngleShift, axis);
            Quaternion endRotation = Quaternion.AngleAxis(m_angle, axis) * startRotation;

            Vector3 virtualStartPosition = p_transform.position + (startRotation * p_transform.forward * m_gizmoRange);
            Vector3 virtualEndPosition = p_transform.position + (endRotation * p_transform.forward * m_gizmoRange);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(p_transform.position, virtualStartPosition);
            Gizmos.DrawLine(p_transform.position, virtualEndPosition);
            Gizmos.DrawSphere(virtualStartPosition, 0.1f);
            Gizmos.DrawSphere(virtualEndPosition, 0.1f);
        }

        private void OnDestroy()
        {
            if (m_target != null)
                m_tween?.Kill();
        }
    }
}
