using UnityEngine;

namespace com.game.player
{
    public class PlayerParanoiaAltarDistanceEffect : MonoBehaviour
    {
        [SerializeField] private PlayerParanoiaLogic m_target;
        [SerializeField] private AnimationCurve m_curve;
        [SerializeField] private float m_coefficient;
        [SerializeField] private float m_minDistance;
        [SerializeField] private float m_maxDistance;

        private void Update()
        {
            Vector3 altarPosition = GetAltarPosition();
            float distance = Vector3.Distance(altarPosition, transform.position);
            float totalRange = m_maxDistance - m_minDistance;

            if (distance < m_minDistance) distance = 0;
            else if (distance > m_maxDistance) distance = m_maxDistance;

            float rawCurveValue = m_curve.Evaluate(distance / totalRange);

            float result = rawCurveValue * m_coefficient;
            m_target.Increase(result);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 altarPosition = Vector3.zero;
            if (!Application.isPlaying) altarPosition = transform.position;
            else altarPosition = GetAltarPosition();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(altarPosition, m_minDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(altarPosition, m_maxDistance);
        }

        // !!!
        Vector3 GetAltarPosition()
        {
            Transform altar = SceneManager.Instance.AltarTransform;

            if (altar == null) 
                return transform.position;

            return SceneManager.Instance.AltarTransform.position;
        }
    }
}
