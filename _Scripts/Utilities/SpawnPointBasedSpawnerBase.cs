using System.Collections.Generic;
using UnityEngine;

namespace com.game.utilities
{
    public abstract class SpawnPointBasedSpawnerBase<T> : SpawnerBase<T> where T : UnityEngine.Object
    {
        [SerializeField] private bool m_spawnPointOccupience = false;
        [SerializeField] private List<Transform> m_spawnPoints = new();

        Dictionary<T, Transform> m_occupiencePairs = new();
        List<Transform> m_occupiedSpawnPoints = new();
        Transform m_lastSelectedSpawnPoint;

        public override void Spawn()
        {
            if (m_spawnPointOccupience && m_occupiedSpawnPoints.Count == m_spawnPoints.Count)
                return;

            base.Spawn();
        }

        protected override void OnSpawnInternal(T spawnCreated)
        {
            base.OnSpawnInternal(spawnCreated);

            if (!m_spawnPointOccupience)
                return;

            if (m_lastSelectedSpawnPoint != null)
            {
                m_occupiencePairs.Add(spawnCreated, m_lastSelectedSpawnPoint);
                m_occupiedSpawnPoints.Add(m_lastSelectedSpawnPoint);
            }

            m_lastSelectedSpawnPoint = null;
        }

        protected override void OnDespawnInternal(T spawnToClear)
        {
            base.OnDespawnInternal(spawnToClear);

            if (!m_spawnPointOccupience)
                return;

            if (!m_occupiencePairs.TryGetValue(spawnToClear, out Transform spawnPointToRelease))
                return;

            m_occupiencePairs.Remove(spawnToClear);

            if (m_occupiedSpawnPoints.Contains(spawnPointToRelease))
                m_occupiedSpawnPoints.Remove(spawnPointToRelease);
        }

        public override Vector3 GetSpawnPoint()
        {
            int randomIndex = Random.Range(0, m_spawnPoints.Count);
            Transform spawnPoint = m_spawnPoints[randomIndex];

            if (m_spawnPointOccupience && m_occupiedSpawnPoints.Contains(spawnPoint))
                return GetSpawnPoint();

            m_lastSelectedSpawnPoint = spawnPoint;

            return spawnPoint.position;
        }
    }
}
