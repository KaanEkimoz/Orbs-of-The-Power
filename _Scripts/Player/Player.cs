using com.absence.attributes;
using com.absence.utilities;
using com.game.player.scriptables;
using UnityEngine;

namespace com.game.player
{
    [DefaultExecutionOrder(-100)]
    public class Player : Singleton<Player>
    {
        [SerializeField, Readonly] private PlayerCharacterProfile m_characterProfile;
        [SerializeField] private PlayerComponentHub m_componentHub;

        private void Update()
        {
            m_componentHub.Inventory.ForceUpdate(); // !!!
        }

        public PlayerComponentHub Hub => m_componentHub;
        public PlayerCharacterProfile CharacterProfile 
        { 
            get
            {
                return m_characterProfile;
            }

            set
            {
                m_characterProfile = value;
            }
        }
    }
}
