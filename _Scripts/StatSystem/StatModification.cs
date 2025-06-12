using System;
using UnityEngine;

namespace com.game.statsystem.presetobjects
{
    [System.Serializable]
    public abstract class StatModification
    {
        public StatModificationType ModificationType;

        [SerializeField, Tooltip("The amount of incremention (eg. +5, -2).")] 
        private float m_incrementalValue;

        [SerializeField, Tooltip("The percentage of incremention (eg. +20%, -25%).")] 
        private float m_percentageValue;

        public float Value
        {
            get
            {
                return ModificationType switch
                {
                    StatModificationType.Incremental => m_incrementalValue,
                    StatModificationType.Percentage => m_percentageValue,
                    _ => -1f,
                };
            }
        }

        public abstract Type GetEnumType();
    }

    [System.Serializable]
    public abstract class StatModification<T> : StatModification where T : System.Enum
    {
        [Tooltip("Target stat.")]
        public T TargetStatType;

        public override Type GetEnumType()
        {
            return typeof(T);
        }
    }
}
