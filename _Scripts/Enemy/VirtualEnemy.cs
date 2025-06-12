using com.game.player;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    public class VirtualEnemy
    {
        private VirtualEnemy(EnemyInstance instance, PlayerLight light)
        {
            instance.AI.Enabled = false;
            instance.SetCollision(false, true);

            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0f;
            randomDirection.Normalize();

            //float randomDistance = Random.Range(light.HalfVisionRadius, light.FullVisionRadius);
            float randomDistance = light.HalfVisionRadius;

            Vector3 lightPosition = light.transform.position;
            lightPosition.y = instance.transform.position.y;

            Vector3 position = lightPosition + (randomDistance * randomDirection);

            Vector3 direction = (lightPosition - position).normalized;
            direction.y = 0f;

            instance.transform.position = position;
            instance.Body.transform.forward = direction;
            instance.HealthBar.gameObject.SetActive(false);
            instance.Combatant.ReinitializeAsFake();
            instance.Spark.Spark();
            instance.Spark.OnEnd += Dispose;

            Enemy.DoVirtualize(instance.Enemy);

            m_instance = instance;
        }

        public static VirtualEnemy CreateNew(EnemyInstance instance, PlayerLight light)
        {
            VirtualEnemy createdEnemy = new VirtualEnemy(instance, light);
            s_list.Add(createdEnemy);

            return createdEnemy;
        }

        public static void KillAll()
        {
            if (s_list == null)
                return;

            foreach (VirtualEnemy instance in s_list)
            {
                instance.Dispose();
            }
        }

        static List<VirtualEnemy> s_list = new();

        EnemyInstance m_instance;

        private void Dispose()
        {
            m_instance.Combatant.Die(DeathCause.Internal);
        }
    }
}
