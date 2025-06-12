using UnityEngine;

namespace com.game.enemysystem.ai
{
    public abstract class EnemyMovementData : ScriptableObject
    {
        public float speed;
        public float stoppingDistance;
    }
}