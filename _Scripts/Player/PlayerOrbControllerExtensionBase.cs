using com.absence.attributes;
using UnityEngine;

namespace com.game.player
{
    public abstract class PlayerOrbControllerExtensionBase : MonoBehaviour
    {
        [SerializeField, Readonly] protected PlayerOrbController m_instance;

        public abstract Vector3 ConvertAimDirection(Vector3 direction);

        private void Reset()
        {
            m_instance = GetComponent<PlayerOrbController>();
        }
    }
}
