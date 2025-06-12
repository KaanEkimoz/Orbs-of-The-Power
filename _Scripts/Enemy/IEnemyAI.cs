using UnityEngine;

namespace com.game.enemysystem.ai
{
    public interface IEnemyAI
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        EnemyAIState State { get; set; }
        EnemyMovementData MovementData { get; }
        bool Enabled { get; set; }
        bool Locked { get; set; }
        float Speed { get; set; }
        float DefaultSpeed { get; }

        void Initialize(Transform target);
        void Refresh(Transform target);
    }
}
