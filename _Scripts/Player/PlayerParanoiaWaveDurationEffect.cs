using com.game.events;
using System;
using UnityEngine;

namespace com.game.player
{
    public class PlayerParanoiaWaveDurationEffect : MonoBehaviour
    {
        [SerializeField] private PlayerParanoiaLogic m_target;
        [SerializeField] private float m_strength;

        bool m_enabled = false;
        bool m_forcedWaveEnded = false;

        private void Start()
        {
            GameEventChannel.OnGameStateChanged += OnGameStateChanged;
            GameEventChannel.OnForcedWaveEnded += OnForcedWaveEnded;
        }

        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            m_forcedWaveEnded = false;
        }

        private void OnForcedWaveEnded()
        {
            m_forcedWaveEnded = true;
        }

        private void Update()
        {
            if (!m_enabled)
                return;

            if (!m_forcedWaveEnded)
                return;

            if (Game.Paused)
                return;

            m_target.Increase(m_strength);
        }
    }
}
