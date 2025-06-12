using DG.Tweening;
using UnityEngine;

namespace com.game.generics
{
    public class MovePingPong : MonoBehaviour
    {
        [SerializeField] private bool m_startOnAwake;
        [SerializeField] private Transform m_target;
        [SerializeField] private Ease m_ease;
        [SerializeField] private Vector3 m_startingShift;
        [SerializeField] private Vector3 m_magnitudePerAxis;
        [SerializeField] private float m_duration;
        [SerializeField] private float m_delay;
        [SerializeField] private int m_loops = -1;

        Transform p_transform => m_target != null ? m_target : transform;

        Tween m_tween;
        Vector3 m_defaultPosition;

        private void Awake()
        {
            p_transform.position = p_transform.position + m_startingShift;

            if (m_startOnAwake)
                Begin();
        }

        public void Begin()
        {
            m_defaultPosition = p_transform.localPosition;

            Vector3 targetPosition = m_defaultPosition + m_magnitudePerAxis;

            m_tween = p_transform.DOLocalMove(targetPosition, m_duration)
                .SetDelay(m_delay)
                .SetEase(m_ease)
                .SetLoops(m_loops, LoopType.Yoyo);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 startPoint = p_transform.position + m_startingShift;
            Vector3 endPoint = startPoint + m_magnitudePerAxis;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint,  endPoint);
            Gizmos.DrawSphere(startPoint, 0.1f);
            Gizmos.DrawSphere(endPoint, 0.1f);
        }

        private void OnDestroy()
        {
            if (m_target != null)
                m_tween?.Kill();
        }
    }
}
