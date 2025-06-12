using com.game.utilities;
using UnityEngine;

namespace com.game.miscs
{
    public class TotemSpawner : SpawnPointBasedSpawnerBase<TotemBehaviour>
    {
        [SerializeField] private bool m_rememberUsedTotems = false;

        protected override void OnDespawn(TotemBehaviour spawnToClear)
        {
            spawnToClear.Dispose();

            if (m_rememberUsedTotems)
                return;

            spawnToClear.OnDispose += () =>
            {
                if (m_spawns.Contains(spawnToClear))
                    m_spawns.Remove(spawnToClear);
            };
        }

        protected override void OnSpawn(TotemBehaviour spawnCreated)
        {

        }
    }
}
