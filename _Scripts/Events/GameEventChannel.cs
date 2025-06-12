using System;
using UnityEngine;

namespace com.game.events
{
    public class GameEventChannel
    {
        public static event Action OnWaveStarted;
        public static void CommitWaveStart() => OnWaveStarted?.Invoke();

        public static event Action OnWaveEnded;
        public static void CommitWaveEnd() => OnWaveEnded?.Invoke();

        public static event Action OnForcedWaveEnded;
        public static void CommitForceWaveEnded() => OnForcedWaveEnded?.Invoke();

        public static event Action<GameState, GameState> OnGameStateChanged;
        public static void CommitGameStateChange(GameState prevState, GameState newState) => OnGameStateChanged?.Invoke(prevState, newState);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            OnWaveStarted = null;
            OnWaveEnded = null;
            OnGameStateChanged = null;
            OnForcedWaveEnded = null;
        }
    }
}
