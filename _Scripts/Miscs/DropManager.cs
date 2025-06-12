using com.absence.utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.miscs
{
    public class DropManager : Singleton<DropManager>
    {
        public const int DEFAULT_MAX_RANDOM_PIECE_COUNT = 10;

        [HideInInspector] public bool Enabled = true;

        [SerializeField] private DropBehaviour m_moneyDropBehaviour;
        [SerializeField] private DropBehaviour m_experienceDropBehaviour;

        public DropBehaviour SpawnMoneyDrop(int amount, Vector3 position)
        {
            DropBehaviour drop = Create(m_moneyDropBehaviour, position, Vector3.zero);

            if (drop == null)
                return null;

            drop.Amount = amount;
            return drop;
        }

        public DropBehaviour SpawnExperienceDrop(int amount, Vector3 position)
        {
            DropBehaviour drop = Create(m_experienceDropBehaviour, position, Vector3.zero);

            if (drop == null)
                return null;

            drop.Amount = amount;
            return drop;
        }

        public List<DropBehaviour> SpawnIndividualMoneyDrops(int amount, Vector3 initialPosition, Action<DropBehaviour> onSpawnForEach = null)
        {
            List<DropBehaviour> result = new();
            for (int i = 0; i < amount; i++) 
            {
                result.Add(SpawnMoneyDrop(1, initialPosition));
            }

            foreach (DropBehaviour drop in result)
            {
                if (drop == null)
                    continue;

                onSpawnForEach?.Invoke(drop);
            }
             
            return result;
        }

        public List<DropBehaviour> SpawnIndividualExperienceDrops(int amount, Vector3 initialPosition, Action<DropBehaviour> onSpawnForEach = null)
        {
            List<DropBehaviour> result = new();
            for (int i = 0; i < amount; i++)
            {
                result.Add(SpawnExperienceDrop(1, initialPosition));
            }

            foreach (DropBehaviour drop in result)
            {
                if (drop == null)
                    continue;

                onSpawnForEach?.Invoke(drop);
            }

            return result;
        }

        public List<DropBehaviour> SpawnRandomlySeperatedMoneyDrops(int amount, Vector3 initialPosition, Action<DropBehaviour> onSpawnForEach = null)
        {
            List<DropBehaviour> result = new();

            if (amount <= 0)
                return result;

            int[] values = com.game.utilities.Helpers.Math.Integers.SeperateRandomly(amount, 1, amount);
            for (int i = 0; i < values.Length; i++)
            {
                result.Add(SpawnMoneyDrop(values[i], initialPosition));
            }

            foreach (DropBehaviour drop in result)
            {
                if (drop == null)
                    continue;

                onSpawnForEach?.Invoke(drop);
            }

            return result;
        }

        public List<DropBehaviour> SpawnRandomlySeperatedExperienceDrops(int amount, Vector3 initialPosition, Action<DropBehaviour> onSpawnForEach = null)
        {
            List<DropBehaviour> result = new();

            if (amount <= 0)
                return result;

            int[] values = com.game.utilities.Helpers.Math.Integers.SeperateRandomly(amount, 1, amount);
            for (int i = 0; i < values.Length; i++)
            {
                result.Add(SpawnExperienceDrop(values[i], initialPosition));
            }

            foreach (DropBehaviour drop in result)
            {
                if (drop == null)
                    continue;

                onSpawnForEach?.Invoke(drop);
            }

            return result;
        }

        T Create<T>(T prefab, Vector3 position, Vector3 eulerAngles) where T : DropBehaviour
        {
            if (!Enabled)
                return null;

            return Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        }
    }
}
