using com.game.enemysystem.ai;
using com.game.generics;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyInstance : MonoBehaviour
    {
        [SerializeField] private Enemy m_enemy;
        [SerializeField] private EnemyCombatant m_combatant;
        [SerializeField] private EnemyAIBuitlInScriptSelector m_AIScriptSelector;
        [SerializeField] private GameObject m_body;
        [SerializeField] private EnemyStats m_stats;
        [SerializeField] private HealthBar m_healthBar;
        [SerializeField] private InterfaceReference<ISpark, MonoBehaviour> m_spark;
        [SerializeField] private List<Collider> m_colliders;

        public Enemy Enemy => m_enemy;
        public EnemyCombatant Combatant => m_combatant;
        public IEnemyAI AI => m_AIScriptSelector.Current;
        public GameObject Body => m_body;
        public EnemyStats Stats => m_stats;
        public HealthBar HealthBar => m_healthBar;
        public ISpark Spark => m_spark.Value;
        public List<Collider> Colliders => m_colliders;

        public void SetCollision(bool value, bool needsGravityDeactivation = false)
        {
            foreach (var collider in Colliders)
            {
                collider.enabled = value;
                if (needsGravityDeactivation && collider.attachedRigidbody != null)
                    collider.attachedRigidbody.useGravity = !value;
            }
        }
    }
}
