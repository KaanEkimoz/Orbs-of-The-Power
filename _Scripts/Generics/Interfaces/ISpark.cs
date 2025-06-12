using System;

namespace com.game.generics
{
    public interface ISpark
    {
        void Spark();
        void ForceStop();

        event Action OnEnd;
    }
}
