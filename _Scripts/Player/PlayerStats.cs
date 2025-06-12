using com.game.player.scriptables;
using UnityEngine;
using com.absence.attributes;
using com.game.player.statsystemextensions;
using System.Collections.Generic;
using System;
using com.game.statsystem;
using com.absence.attributes.experimental;

namespace com.game.player
{
    /// <summary>
    /// The PlayerComponent responsible for managing anything related to player stats.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class PlayerStats : MonoBehaviour, IStats<PlayerStatType>
    {
        [Header("Utilities")]

        [SerializeField, Tooltip("If enabled, this component with initialize itself, and also some additional console messages will take place.")]
        private bool m_debugMode = false;

        [SerializeField, Required, InlineEditor, Tooltip("Default values provided for the any initialization process.")]
        private PlayerDefaultStats m_defaultStats;

        [SerializeField, ShowIf(nameof(m_debugMode)), Required, Tooltip("Profile provided for the self-initialization process.")]
        private PlayerCharacterProfile m_defaultCharacterProfile;

        [Space, SerializeField, HelpBox("This field might be required for some modifiers to take place.", HelpBoxType.Info)] 
        private PlayerStatPipeline m_statPipeline;

        [Header("Stats")]
        [SerializeField, Readonly] private PlayerStatHolder m_statHolder;

        Dictionary<PlayerStatType, float> m_defaultValues;

        public IStatManipulator<PlayerStatType> Manipulator => m_statHolder;

        public StatPipeline<PlayerStatType> Pipeline => m_statPipeline;
        public Dictionary<PlayerStatType, float> DefaultValues => m_defaultValues;

        private void Awake()
        {
            Initialize(m_defaultStats);
            if (m_debugMode) ApplyCharacterProfile(m_defaultCharacterProfile);

            if (Pipeline != null) Pipeline.Refresh();
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with default values for each stat.
        /// </summary>
        /// <param name="defaultValues">The default values provided.</param>
        public void Initialize(PlayerDefaultStats defaultValues)
        {
            m_statHolder = new PlayerStatHolder(defaultValues);
            //Debug.Log("PlayerStats initialized.");
        }

        /// <summary>
        /// Use to apply a character profile to the player stats.
        /// </summary>
        /// <param name="profile">The character profile provided.</param>
        public void ApplyCharacterProfile(PlayerCharacterProfile profile)
        {
            m_statHolder.ApplyCharacterProfile(profile);
            m_defaultValues = new();

            FillDefaultValues();

            Player.Instance.CharacterProfile = profile;
        }

        public float GetStat(PlayerStatType targetStat)
        {
            if (!Application.isPlaying)
                return m_defaultStats.GetDefaultValue(targetStat);

            float rawStatValue = m_statHolder.GetStat(targetStat);

            if (Pipeline == null)
            {
                Debug.LogWarning("Player stat pipeline is null.");
                return rawStatValue;
            }

            return Pipeline.Process(targetStat, rawStatValue);
        }

        public PlayerStatHolder GetRawStatHolder()
        {
            return m_statHolder;
        }

        #endregion

        void FillDefaultValues()
        {
            foreach (PlayerStatType enumValue in Enum.GetValues(typeof(PlayerStatType)))
            {
                m_defaultValues.Add(enumValue, m_statHolder.GetStat(enumValue));
            }
        }
    }
}