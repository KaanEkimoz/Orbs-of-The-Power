using System;
using UnityEngine;

namespace com.game
{
    public static class Game
    {
        public static bool Paused { get; private set; }
        public static bool Initialized { get; set; }

        public static GameRuntimeEvent Event { get; set; } = GameRuntimeEvent.Null;

        public static event Action OnPause = null;
        public static event Action OnResume = null;

        public static void Pause()
        {
            if (Paused) return;

            Time.timeScale = 0f;
            Cursor.visible = true;
            Paused = true;

            OnPause?.Invoke();
        }

        public static void Resume(float overrideTimeScale = 1f)
        {
            if (!Paused) return;

            Time.timeScale = overrideTimeScale;
            Cursor.visible = false;
            Paused = false;

            OnResume?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Reset()
        {
            Initialized = false;
            Paused = true;
            OnPause = null;
            OnResume = null;

            Resume();
        }
    }
}
