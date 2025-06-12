using UnityEngine;

namespace com.game.enemysystem.ai
{
    [CreateAssetMenu(fileName = "New Polarith Movement Data", menuName = "Game/Enemy System/AI/Movement Data (PolarithAI)", order = int.MinValue)]
    public class EnemyMovementDataPolarith : EnemyMovementData
    {
        public float angularSpeed;
    }
}
