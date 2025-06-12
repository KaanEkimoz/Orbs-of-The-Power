using com.absence.attributes;
using com.absence.utilities;
using com.game.events;
using System;
using UnityEngine;

namespace com.game
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : Singleton<GameManager>
    {
        [Header1("Game Manager")]

        [Header2("Runtime")]

        [SerializeField, Readonly] private GameState m_state = GameState.NotStarted;

        public GameState State => m_state;
        public event Action<GameState, GameState> OnStateChanged;

        public bool SetState(GameState newState, bool force = false)
        {
            if ((!force) && m_state.Equals(newState))
                return false;

            DoSetState(newState);
            return true;
        }

        void DoSetState(GameState newState)
        {
            GameState prevState = m_state;
            m_state = newState;

            OnStateChanged?.Invoke(prevState, m_state);
            GameEventChannel.CommitGameStateChange(prevState, m_state);
        }
    }
}
