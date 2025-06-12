using UnityEngine;

namespace com.game.generics 
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public enum CallMode
        {
            OnEnable,
            Awake,
            Start,
            Manual,
        }

        [SerializeField] private CallMode m_callMode = CallMode.Start;
        [SerializeField, Min(0f)] private float m_timeDelay = 1f;

        bool m_invoked;

        public bool Invoked => m_invoked;

        public CallMode Mode
        {
            get
            {
                return m_callMode;
            }

            set
            {
                m_callMode = value;
            }
        }

        public float TimeDelay
        {
            get
            {
                return m_timeDelay;
            }

            set
            {
                m_timeDelay = value;
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            if (m_callMode == CallMode.OnEnable)
                DOInvoke();
        }

        private void Awake()
        {
            if (m_callMode == CallMode.Awake)
                DOInvoke();
        }

        private void Start()
        {
            if (m_callMode == CallMode.Start)
                DOInvoke();
        }

        public void Invoke()
        {
            if (m_callMode != CallMode.Manual)
            {
                Debug.Log("This DestroyAfterTime component is not manual!");
                return;
            }

            DOInvoke();
        }

        void DOInvoke()
        {
            if (m_invoked)
                return;

            Destroy(gameObject, m_timeDelay);
        }
    }
}
