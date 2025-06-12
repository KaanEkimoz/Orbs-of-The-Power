using com.game.player;
using UnityEngine;

namespace com.game.utilities
{
    public abstract class PlayerRelatedSpawnerBase<T> : SpawnerBase<T> where T : Object
    {
        [Space]
        
        [SerializeField] private LayerMask m_includeMasks;
        [SerializeField] private LayerMask m_excludeMask;
        [SerializeField] private float m_groundCheckPlaneRadius;
        [SerializeField] private Vector2 m_playerDistanceRange;

        Player m_player;

        private void Start()
        {
            m_player = Player.Instance;
        }

        public override Vector3 GetSpawnPoint()
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle;
            Vector3 randomXZ = new Vector3(randomCircle.x, 0f, randomCircle.y);

            float magnitude = UnityEngine.Random.Range(m_playerDistanceRange.x, m_playerDistanceRange.y);
            Vector3 calculatedPosition = m_player.transform.position + (randomXZ * magnitude);

            if (!Helpers.Physics.TryGetGroundSnappedPosition(calculatedPosition, m_groundCheckPlaneRadius, m_includeMasks, out _))
                return GetSpawnPoint();

            if (Helpers.Physics.TryGetGroundSnappedPosition(calculatedPosition, m_groundCheckPlaneRadius, m_excludeMask, out _))
                return GetSpawnPoint();

            calculatedPosition.y = 0f;
            return calculatedPosition;
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_playerDistanceRange.x);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_playerDistanceRange.y);
            Gizmos.DrawWireSphere(transform.position, m_groundCheckPlaneRadius);
        }
    }
}
