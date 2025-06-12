using System;

namespace com.game.utilities
{
    public interface IStunnable : IObject
    {
        public event Action<CCBreakFlags> OnStun;

        void Stun(float duration, CCSource source, CCBreakFlags flags);
    }
}
