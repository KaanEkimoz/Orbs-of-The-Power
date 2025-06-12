using com.absence.attributes;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace com.game.statsystem
{
    public abstract class StatPipelineComponentBase<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] protected int m_order = 0;
        [Space, SerializeField]
        protected List<StatPipelineStatEntry<T>> m_statEntries = new();

        Dictionary<T, float> m_coefficientPairs;

        public int Order => m_order;

        public void Initialize()
        {
            RefreshCoefficients();
            Initialize_Internal();
        }

        public float Process(T statType, float rawValue)
        {
            float statCoefficient = 1f;
#if !UNITY_EDITOR
            if (!m_coefficientPairs.TryGetValue(statType, out statCoefficient))
                return rawValue;
#else
            StatPipelineStatEntry<T> entry = m_statEntries.Find(eny => eny.TargetStat.Equals(statType));
            if (entry == null)
                return rawValue;
            else
                statCoefficient = entry.Coefficient;
#endif

            return Process_Internal(statType, statCoefficient, rawValue);
        }

        protected abstract void Initialize_Internal();
        protected abstract float Process_Internal(T statType, float statCoefficient, float rawValue);

        public virtual void OnTestGUI()
        {
        }

        [Button("Validate Entries")]
        public void Validate()
        {
            StringBuilder sb = new("Result of the validation:\n\n");

            bool error = false;
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                List<StatPipelineStatEntry<T>> entryList =
                    m_statEntries.FindAll(entry => entry.TargetStat.Equals(enumValue));

                int count = entryList.Count;

                if (count > 1)
                {
                    sb.Append($"There are multiple ({count}) entries of the stat: '{enumValue}' found.\n");
                    error = true;
                }
            }

            if (error)
            {
                Debug.LogWarning(sb.ToString());
                return;
            }

            Debug.LogWarning("No problems found.");
        }

        [Button("Refresh Coefficients")]
        public virtual void RefreshCoefficients()
        {
            if (!Application.isPlaying) return;

            m_coefficientPairs = new();

#if !UNITY_EDITOR
            m_statEntries.ForEach(entry =>
            {
                if (!m_coefficientPairs.ContainsKey(entry.TargetStat))
                    m_coefficientPairs.Add(entry.TargetStat, entry.Coefficient);
            });
#endif
        }
    }
}
