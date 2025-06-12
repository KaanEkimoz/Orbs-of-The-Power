using com.game.statsystem.internals;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.statsystem
{
    public interface IDefaultStats<T> where T : Enum
    {
        float GetDefaultValue(T targetStat);
        bool TryGetDefaultValue(T targetStat, out float defaultValue);
    }

    public abstract class DefaultStats<T> : DefaultStats, IDefaultStats<T> where T : Enum
    {
        [SerializeField] private List<StatDefaultValue<T>> m_defaultValues = null;
        [SerializeField] private bool m_initialized = false;

        public int Length => m_defaultValues.Count;

        public override Type GetEnumType()
        {
            return typeof(T);
        }

        public override void Reinitialize()
        {
            List<StatDefaultValue<T>> cache = null;

            if (m_defaultValues == null) cache = new();
            else cache = new(m_defaultValues);

            m_defaultValues = new();

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                StatDefaultValue<T> value = cache.Find(dv => dv.TargetStat.Equals(enumValue));
                if (value == null) m_defaultValues.Add(new StatDefaultValue<T>(enumValue, 0f));
                else m_defaultValues.Add(value);
            }

            m_initialized = true;
        }

        public float GetDefaultValue(T targetStat)
        {
            return m_defaultValues.Find(dv => dv.TargetStat.Equals(targetStat)).Value;
        }

        public bool TryGetDefaultValue(T targetStat, out float defaultValue)
        {
            defaultValue = 0f;

            StatDefaultValue<T> valueObject = m_defaultValues.FirstOrDefault(dv => dv.TargetStat.Equals(targetStat));
            if (valueObject == null) return false;

            defaultValue = valueObject.Value;
            return true;
        }
    }
}
