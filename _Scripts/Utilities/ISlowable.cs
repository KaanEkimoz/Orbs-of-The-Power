using System;

namespace com.game.utilities
{
    public interface ISlowable : IObject
    {
        public event Action<float, float, CCSource, CCBreakFlags> OnSlow;

        public void SlowForSeconds(float slowPercent, float duration, CCSource source, CCBreakFlags flags);
    }
}
