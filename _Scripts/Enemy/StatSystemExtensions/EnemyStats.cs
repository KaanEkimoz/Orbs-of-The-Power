using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.enemysystem.statsystemextensions;
using com.game.statsystem;
using UnityEngine;

namespace com.game.enemysystem
{
    [DefaultExecutionOrder(-100)]
    public class EnemyStats : MonoBehaviour, IStats<EnemyStatType>
    {
        [Header("Utilities")]

        [SerializeField, Required, InlineEditor, Tooltip("Default values provided for the any initialization process.")]
        private EnemyDefaultStats m_defaultStats;

        [Header("Stats")]
        [SerializeField, Readonly] private EnemyStatHolder m_statHolder;

        [SerializeField]
        private EnemyStatPipeline m_statPipeline;

        public IStatManipulator<EnemyStatType> Manipulator => m_statHolder;
        public StatPipeline<EnemyStatType> Pipeline => m_statPipeline;

        private void Awake()
        {
            Initialize(m_defaultStats);
            //if (m_debugMode) ApplyCharacterProfile(m_defaultCharacterProfile);

            if (Pipeline != null) Pipeline.Refresh();
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with default values for each stat.
        /// </summary>
        /// <param name="defaultValues">The default values provided.</param>
        public void Initialize(EnemyDefaultStats defaultValues)
        {
            m_statHolder = new EnemyStatHolder(defaultValues);
        }

        public float GetStat(EnemyStatType targetStat)
        {
            float rawStatValue = m_statHolder.GetStat(targetStat);

            if (Pipeline == null)
            {
                Debug.LogWarning("Player stat pipeline is null.");
                return rawStatValue;
            }

            return Pipeline.Process(targetStat, rawStatValue);
        }

        #endregion
    }
}
