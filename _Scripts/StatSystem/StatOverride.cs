using System;
using UnityEngine;

namespace com.game.statsystem.presetobjects
{
    [System.Serializable]
    public abstract class StatOverride
    {
        [Tooltip("The intended value for the target stat to have after the operation.")]
        public float NewValue;

        public abstract Type GetEnumType();
    }

    [System.Serializable]
    public abstract class StatOverride<T> : StatOverride where T : System.Enum
    {
        [Tooltip("Target stat.")]
        public T TargetStatType;

        public override Type GetEnumType()
        {
            return typeof(T);
        }
    }
}
