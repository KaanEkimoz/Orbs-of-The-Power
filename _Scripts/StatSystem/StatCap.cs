using System;
using UnityEngine;

namespace com.game.statsystem.presetobjects
{
    [System.Serializable]
    public abstract class StatCap
    {
        [Tooltip("If enabled, target stat will have a min limit.")] 
        public bool CapLow;

        [Tooltip("If enabled, target stat will have a max limit.")] 
        public bool CapHigh;

        [Tooltip("The min value allowed for the target stat to have.")]
        public float MinValue;

        [Tooltip("The max value allowed for the target stat to have.")] 
        public float MaxValue;

        public abstract Type GetEnumType();
    }

    [System.Serializable]
    public abstract class StatCap<T> : StatCap where T : Enum
    {
        [Tooltip("Target stat.")]
        public T TargetStatType;

        public override Type GetEnumType()
        {
            return typeof(T);
        }
    }
}
