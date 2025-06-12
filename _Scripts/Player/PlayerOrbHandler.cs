using com.game.player.scriptables;
using UnityEngine;

namespace com.game.player
{
    public class PlayerOrbHandler : MonoBehaviour
    {
        int m_orbsInHand;
        int m_maxOrbsInHand;

        public int OrbsInHand => m_orbsInHand;
        public int MaxOrbsCanBeHeld => m_maxOrbsInHand;

        public bool OrbsAtMax => m_orbsInHand >= m_maxOrbsInHand;
        public bool OrbsAtMin => m_orbsInHand <= 0;

        PlayerCharacterProfile m_characterProfile;

        private void Awake()
        {
            m_characterProfile = Player.Instance.CharacterProfile;
            m_orbsInHand = m_characterProfile.OrbCount;
            m_maxOrbsInHand = Constants.Gameplay.MAX_ORBS_CAN_BE_HELD;
        }

        public void AddOrb()
        {
            if (OrbsAtMax) return;

            m_orbsInHand++;
        }

        public void RemoveOrb()
        {
            if (OrbsAtMin) return;

            m_orbsInHand--;
        }
    }
}
