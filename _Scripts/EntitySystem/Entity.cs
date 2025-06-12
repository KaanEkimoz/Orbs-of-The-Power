using com.game.generics.interfaces;
using UnityEngine;

namespace com.game.entitysystem
{
    public class Entity : MonoBehaviour, ITakeDamage
    {
        [SerializeField] private bool m_initializeOnStart = true;
        [SerializeField] EntityComponentHub m_componentHub;

        [SerializeField] IEntityStatProvider m_statProvider;

        public EntityComponentHub Hub => m_componentHub;

        private void Start()
        {
            AutoInitialize();
        }

        private void AutoInitialize()
        {
            if (TryGetComponent(out IEntityStatProvider statProvider))
                SetStatProvider(statProvider);
            else
                Debug.LogWarning("IEntityStatProvider needed for auto-initialization of an Entity script to work.");
        }

        public void SetStatProvider(IEntityStatProvider provider)
        {
            m_statProvider = provider;
        }

        public void TakeDamage(float amount)
        {
            
        }
    }
}
