using com.game.player;
using System;
using UnityEngine;

namespace com.game.events
{
    public static class PlayerEventChannel
    {
        public static event Action<PlayerLevelingLogic> OnLevelUp;
        public static void CommitLevelUp(PlayerLevelingLogic instance) => OnLevelUp?.Invoke(instance);

        public static event Action OnEnemyKill;
        public static void CommitEnemyKill() => OnEnemyKill?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            OnLevelUp = null;
            OnEnemyKill = null;
        }
    }
}
