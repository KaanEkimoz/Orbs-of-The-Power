using UnityEngine;

namespace com.game.generics
{
    public class LockUp : MonoBehaviour
    {
        [SerializeField] private bool m_enabled = true;

        public bool Enabled
        {
            get
            {
                return m_enabled;
            }

            set
            {
                m_enabled = value;
            }
        }

        private void Update()
        {
            if (m_enabled)
                transform.up = Vector3.up;
        }
    }
}
