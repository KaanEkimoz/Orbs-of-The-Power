using UnityEngine;

namespace com.game.utilities
{
    public interface IKnockbackable : IObject
    {
        public void Knockback(Vector3 direction, float strength, KnockbackSourceUsage usage);
    }
}
