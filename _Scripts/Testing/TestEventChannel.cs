using System;
using UnityEngine;

namespace com.game.testing
{
    public static class TestEventChannel
    {
        public static event Action OnEnemyKilled;
        public static void ReceiveEnemyKill() => OnEnemyKilled?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Reset()
        {
            OnEnemyKilled = null;
        }
    }
}
