using com.absence.attributes;
using com.game.enemysystem;
using UnityEngine;
using UnityEngine.Events;

namespace com.game.complementaries
{
    public class DamageableEffects : MonoBehaviour
    {
        [SerializeField, Required] private EnemyCombatant m_target;

        public UnityEvent<float> onTakeDamage;
        public UnityEvent<float> onHeal;
        public UnityEvent<DeathCause> onDie;

        private void Start()
        {
            m_target.OnTakeDamage += OnTakeDamage;
            m_target.OnDie += OnDie;
            m_target.OnHeal += OnHeal;
        }

        private void OnHeal(float amount)
        {
            onHeal.Invoke(amount);
        }

        private void OnDie(DeathCause cause)
        {
            onDie.Invoke(cause);
        }

        private void OnTakeDamage(float amount)
        {
            onTakeDamage.Invoke(amount);
        }
    }
}
