using com.game.player;
using com.game.utilities;

namespace com.game.enemysystem
{
    public class EnemySpawner : PlayerRelatedSpawnerBase<EnemyInstance>
    {
        protected override void OnSpawn(EnemyInstance spawnCreated)
        {
            EnemyCombatant enemyCombatant = spawnCreated.Combatant;
            enemyCombatant.ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
            enemyCombatant.OnDie += (_) =>
            {
                if (m_spawns.Contains(spawnCreated))
                    m_spawns.Remove(spawnCreated);
            };
        }

        protected override void OnDespawn(EnemyInstance spawnToClear)
        {
            spawnToClear.Combatant.Die(DeathCause.Internal);
        }
    }
}