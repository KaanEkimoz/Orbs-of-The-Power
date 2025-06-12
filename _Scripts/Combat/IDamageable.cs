using com.game.utilities;
using System;
using UnityEngine;

namespace com.game
{
    public interface IDamageable : IObject
    {
        bool IsAlive { get; }
        float Health { get; }
        float MaxHealth { get; }

        event Action<float> OnTakeDamage;
        event Action<float> OnHeal;
        event Action<DeathCause> OnDie;

        void TakeDamage(float damage, out DamageEvent evt);
        void Heal(float amount);

        void Die(DeathCause cause);
    }

    public interface IRenderedDamageable : IDamageable
    {
        Renderer Renderer { get; }
    }
}
