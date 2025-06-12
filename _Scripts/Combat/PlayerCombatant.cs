using com.game.enemysystem.statsystemextensions;
using com.game.player.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace com.game.player
{
    public class PlayerCombatant : MonoBehaviour, IRenderedDamageable
    {
        [SerializeField] private GameObject m_container;
        [SerializeField] private Renderer m_renderer;

        float _health;
        float _maxHealth;
        PlayerStats _playerStats;

        public bool IsAlive => _health > 0;
        public float Health => _health;
        public float MaxHealth => _maxHealth;
        public Renderer Renderer => m_renderer;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action<float> OnHeal = delegate { };
        public event Action<DeathCause> OnDie = delegate { };

        [Inject] PlayerOrbController _orbController;

        private void Awake()
        {
            if(_playerStats == null)
                _playerStats = GetComponent<PlayerStats>();

            _maxHealth = _playerStats.GetStat(PlayerStatType.Health);
            _health = _maxHealth;
        }
        public void OnLifeSteal(float amount)
        {
            Heal(amount * (_playerStats.GetStat(PlayerStatType.LifeSteal) / 100));
        }
        public void TakeDamage(float damage, out DamageEvent evt)
        {
            if (damage == 0f)
            {
                evt = DamageEvent.Empty;
                return;
            }

            float damageDealt = damage * (1 - (_playerStats.GetStat(PlayerStatType.Armor) / 100));

            _health -= damageDealt;

            bool causedDeath = _health <= 0;

            if (causedDeath)
            {
                _health = 0;
                Die(DeathCause.Default);
            }

            evt = new DamageEvent()
            {
                DamageSent = damage,
                DamageDealt = damageDealt,
                CausedDeath = causedDeath,
            };

            OnTakeDamage?.Invoke(damage);
        }
        public void HealWithLifeSteal(float amount)
        {
            Heal(amount * (_playerStats.GetStat(PlayerStatType.LifeSteal) / 100));
        }
        public void Heal(float amount)
        {
            _health += amount;

            if (_health > _maxHealth)
                _health = _maxHealth;

            OnHeal?.Invoke(amount);
        }
        public void Die(DeathCause cause)
        {
            if (m_container != null) Destroy(m_container);
            else Destroy(gameObject);

            OnDie?.Invoke(cause);
        }
    }
}
