using UnityEngine;

namespace com.game.player
{
    public class PlayerOrbControllerParanoiaExtension : PlayerOrbControllerExtensionBase
    {
        [SerializeField] private float m_strength;

        PlayerParanoiaLogic m_paranoia;

        private void Start()
        {
            m_paranoia = Player.Instance.Hub.Paranoia;
        }

#pragma warning disable CS0162 // Unreachable code detected
        public override Vector3 ConvertAimDirection(Vector3 direction)
        {
            int axis = 0;
            if (InternalSettings.AIM_PARANOIA_CAN_BYPASS)
            {
                axis = Random.Range(-1, 2);
            }

            else
            {
                int random = Random.Range(0, 2);
                if (random == 0) axis = -1;
                else if (random == 1) axis = 1;
            }

            float strength = Random.Range(0f, m_strength);
            float magnitude = strength * (float)axis;
            float angle = magnitude * (float)m_paranoia.SegmentIndex;

            // ????
            Vector3 result = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            return result;
        }
#pragma warning restore CS0162 // Unreachable code detected
    }
}
