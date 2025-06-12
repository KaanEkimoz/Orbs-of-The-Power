using com.absence.attributes;
using com.absence.timersystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.utilities
{
    public interface ISpawner 
    {
        bool ControlledBySceneManager { get; set; }
        bool IsActive { get; set; }
        bool IsPaused { get; set; }

        void Clear();
    }

    public abstract class SpawnerBase : MonoBehaviour, ISpawner
    {
        public abstract bool ControlledBySceneManager { get; set; }
        public abstract bool IsActive { get; set; }
        public abstract bool IsPaused { get; set; }

        public abstract void Clear();
    }

    public abstract class SpawnerBase<T> : SpawnerBase, ISpawner where T : UnityEngine.Object
    {
        [SerializeField] private bool m_controlledBySceneManager = true;
        [SerializeField] private bool m_startActive = true;
        [SerializeField] private bool m_prewarm = false;
        [SerializeField] private bool m_spawnExceededBurst = false;
        [SerializeField, Min(1)] private int m_burstCount = 1;
        [SerializeField, Min(0)] private int m_maxSpawnsCanExist;
        [SerializeField, MinMaxSlider(0f, 100f)] private Vector2 m_baseSpawnIntervalRange;
        [SerializeField, Readonly] private bool m_active = false;
        [SerializeField, Readonly] private bool m_paused = false;

        [Space]

        [SerializeField] private List<T> m_prefabs;

        public event Action<T> onSpawn;
        public event Action onClear;

        protected List<T> m_spawns;
        protected ITimer m_timer;

        public override bool ControlledBySceneManager
        {
            get
            {
                return m_controlledBySceneManager;
            }

            set
            {
                m_controlledBySceneManager = value;
            }
        }

        public override bool IsActive
        {
            get
            {
                return m_active;
            }

            set
            {
                if (m_active == value)
                    return;

                m_active = value;

                if (m_active)
                {
                    if (m_prewarm)
                        Spawn();

                    ResetTimer();
                }

                else
                {
                    m_timer?.Fail();
                }
            }
        }

        public override bool IsPaused
        {
            get
            {
                return m_paused;
            }

            set
            {
                if (m_paused == value)
                    return;

                m_paused = value;

                if (m_paused) m_timer?.Pause();
                else m_timer?.Resume();
            }
        }

        protected virtual void Awake()
        {
            m_spawns = new();

            if (m_startActive)
                IsActive = true;
        }

        void ResetTimer()
        {
            m_timer?.Fail();
            DoResetTimer();
        }

        void DoResetTimer()
        {
            if (!IsActive)
                return;

            float baseInterval = UnityEngine.Random
                .Range(m_baseSpawnIntervalRange.x, m_baseSpawnIntervalRange.y);

            m_timer = Timer.Create(baseInterval)
                .OnComplete(OnIntervalTimerComplete);

            if (IsPaused)
                m_timer.Pause();
        }

        private void OnIntervalTimerComplete(TimerCompletionContext context)
        {
            m_timer = null;

            if (!context.Succeeded)
                return;

            Spawn();
            DoResetTimer();
        }

        public abstract Vector3 GetSpawnPoint();
        protected abstract void OnSpawn(T spawnCreated);
        protected abstract void OnDespawn(T spawnToClear);

        protected virtual void OnSpawnInternal(T spawnCreated)
        {
            OnSpawn(spawnCreated);
        }

        protected virtual void OnDespawnInternal(T spawnToClear)
        {
            OnDespawn(spawnToClear);
        }

        public virtual void Spawn()
        {
            int spawnCount = m_burstCount;
            int diff = m_maxSpawnsCanExist - m_spawns.Count;

            if (diff == m_burstCount)
                return;

            if (diff < m_burstCount)
            {
                if (m_spawnExceededBurst)
                    spawnCount -= diff;
                else
                    return;
            }

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPoint = GetSpawnPoint();
                T prefab = GetPrefab();

                T spawn = Instantiate(prefab, spawnPoint, Quaternion.identity);
                OnSpawnInternal(spawn);

                m_spawns.Add(spawn);

                onSpawn?.Invoke(spawn);
            }
        }

        public virtual T GetPrefab()
        {
            int randomIndex = UnityEngine.Random.Range(0, m_prefabs.Count);
            T prefab = m_prefabs[randomIndex];

            return prefab;
        }

        public override void Clear()
        {
            if (m_spawns == null)
            {
                onClear?.Invoke();
                return;
            }

            foreach (T spawn in m_spawns)
            {
                if (spawn != null)
                    OnDespawnInternal(spawn);
            }

            m_spawns.Clear();

            onClear?.Invoke();
        }
    }
}
