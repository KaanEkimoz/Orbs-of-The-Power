using System;

namespace com.game.statsystem
{
    [System.Serializable]
    public class StatPipelineStatEntry<T> where T : Enum
    {
        public T TargetStat;
        public float Coefficient;
    }
}
