using UnityEngine;

namespace com.game.enemysystem.ai
{
    [CreateAssetMenu(fileName = "New NavMeshAgent Movement Data", menuName = "Game/Enemy System/AI/Movement Data (NavMeshAgent)", order = int.MinValue)]
    public class EnemyMovementDataNavMeshAgent : EnemyMovementData
    {
        public float angularSpeed;
        public float acceleration;
    }
}
