using com.game.events;
using System;
using UnityEngine;

namespace com.game.miscs
{
    public class DropBehaviour : MonoBehaviour, IGatherable, IMagnetable
    {
        [Header("Utilities")]

        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private GameObject m_container;

        [Space, Header("Settings")]

        [SerializeField] private int m_amount = 0;
        [SerializeField] private bool m_applyForceOnSpawn = false;

        protected Func<DropBehaviour, IGatherer, bool> m_onGather;

        Vector3 m_spawnForce;
        bool m_spawnForceApplied = false;
        bool m_magnetable = true;
        bool m_gatherable = true;
        bool m_destroyOnGather = true;

        public virtual bool ApplySpawnForce
        {
            get
            {
                return m_applyForceOnSpawn;
            }

            set
            {
                m_applyForceOnSpawn = value;
            }
        }

        public virtual bool IsGatherable
        {
            get
            {
                return m_gatherable;
            }

            protected set 
            {
                m_gatherable = value;
            }
        }

        public virtual bool IsMagnetable
        {
            get
            {
                return m_magnetable;
            }

            protected set
            {
                m_magnetable = value;
            }
        }

        public virtual bool DestroyOnGather
        {
            get
            {
                return m_destroyOnGather;
            }

            set
            {
                m_destroyOnGather = value;
            }
        }

        public virtual int Amount
        {
            get
            {
                return m_amount;
            }

            set
            {
                m_amount = value;
            }
        }

        public virtual float MagnetResistance => 0f;
        public virtual bool AllowNonPlayerGatherers => false;

        protected virtual void Start()
        {
            SubscribeToEvents();
            ApplySpawnForceIfNotApplied();
        }

        void SubscribeToEvents()
        {
            GameEventChannel.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            Destroy();
        }

        void ApplySpawnForceIfNotApplied()
        {
            if (!ApplySpawnForce)
                return;

            if (m_spawnForceApplied)
                return;

            m_rigidbody.AddForce(m_spawnForce, ForceMode.Impulse);
            m_spawnForceApplied = true;
        }

        public void SetSpawnForce(Vector3 force)
        {
            ApplySpawnForce = true;
            m_spawnForce = force;
        }

        public void SetSpawnForce(Vector3 direction, float magnitude)
        {
            SetSpawnForce(direction * magnitude);
        }

        public bool TryGather(IGatherer sender)
        {
            if ((!AllowNonPlayerGatherers) && (!sender.IsPlayer))
                return false;

            bool result = true;

            if (!OnGather(sender)) 
                result = false;

            if (m_onGather != null && (!m_onGather.Invoke(this, sender))) 
                result = false;

            if (result && DestroyOnGather)
                Destroy();

            return result;
        }

        public void OnGather(Func<DropBehaviour, IGatherer, bool> onGather)
        {
            m_onGather = onGather;
        }

        protected virtual bool OnGather(IGatherer sender)
        {
            return true;
        }

        public void Destroy()
        {
            if (m_container != null) Destroy(m_container);
            else Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IGatherer gatherer))
                return;

            TryGather(gatherer);
        }
    }
}
