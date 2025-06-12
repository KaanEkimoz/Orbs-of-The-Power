using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.statsystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.orbsystem.statsystemextensions
{
    public class OrbStats : MonoBehaviour, IStats<OrbStatType>
    {
        [Header("Utilities")]

        [SerializeField, Required, InlineEditor, Tooltip("Default values provided for the any initialization process.")]
        private OrbDefaultStats m_defaultStats;

        [Header("Stats")]
        [SerializeField, Readonly] private OrbStatHolder m_statHolder;

        Dictionary<OrbStatType, float> m_defaultValues;

        public IStatManipulator<OrbStatType> Manipulator => m_statHolder;
        public StatPipeline<OrbStatType> Pipeline => null;
        public Dictionary<OrbStatType, float> DefaultValues => m_defaultValues;

        private void Awake()
        {
            Initialize(m_defaultStats);
            //if (m_debugMode) ApplyCharacterProfile(m_defaultCharacterProfile);
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with default values for each stat.
        /// </summary>
        /// <param name="defaultValues">The default values provided.</param>
        public void Initialize(OrbDefaultStats defaultValues)
        {
            m_statHolder = new OrbStatHolder(defaultValues);

            FillDefaultValues();

            //Debug.Log("OrbStats initialized.");

            if (Pipeline != null) Pipeline.Refresh();
        }

        #endregion

        void FillDefaultValues()
        {
            m_defaultValues = new();

            foreach (OrbStatType enumValue in Enum.GetValues(typeof(OrbStatType)))
            {
                m_defaultValues.Add(enumValue, m_statHolder.GetStat(enumValue));
            }
        }

        public float GetStat(OrbStatType targetStat)
        {
            if (!Application.isPlaying)
                return m_defaultStats.GetDefaultValue(targetStat);

            float rawStatValue = m_statHolder.GetStat(targetStat);

            if (Pipeline == null)
            {
                Debug.LogWarning("Orb stat pipeline is null.");
                return rawStatValue;
            }

            return Pipeline.Process(targetStat, rawStatValue);
        }
    }
}
