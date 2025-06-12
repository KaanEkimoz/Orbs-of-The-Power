using com.game.events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerLevelingLogic : MonoBehaviour
    {
        [SerializeField] private List<int> m_placeholder = new();

        public int CurrentLevel => m_currentLevel;
        public int CurrentExperience => m_currentExperience;
        public int ExperienceNeededForNextLevel => m_targetExperience;
        public float CurrentExperienceRatio => m_currentExperience / m_targetExperience;
        public int LastLevelGain => m_lastLevelGain;

        public event Action<PlayerLevelingLogic> OnLevelUp;
        public event Action<int> OnGainExperience;

        int m_currentLevel;
        int m_currentExperience;
        int m_targetExperience;
        int m_lastLevelGain;

        private void Awake()
        {
            m_currentLevel = 1;
            m_targetExperience = 10;
        }

        public bool GainExperience(int amount)
        {
            if (amount <= 0)
                return LoseExperience(-amount);

            m_currentExperience += amount;

            int m_levelsGained = 0;
            while (m_currentExperience >= m_targetExperience)
            {
                m_currentExperience -= m_targetExperience;
                m_levelsGained++;
            }

            OnGainExperience?.Invoke(amount);
            LevelUp(m_levelsGained);

            return m_levelsGained > 0;
        }

        public bool LoseExperience(int amount)
        {
            if (amount < 0)
                return GainExperience(-amount);

            m_currentExperience -= amount;
            bool causedNegative = m_currentExperience < 0;

            m_currentExperience = Mathf.Max(0, m_currentExperience);

            if (causedNegative)
                return false;

            return true;
        }

        public void LevelUp(int amount)
        {
            if (amount <= 0)
                return;

            m_lastLevelGain = amount;
            m_currentLevel += amount;
            m_targetExperience = m_placeholder[m_currentLevel - 1];
            OnLevelUp?.Invoke(this);
            PlayerEventChannel.CommitLevelUp(this);
        }

        public void LevelUp()
        {
            LevelUp(1);
        }
    }
}
