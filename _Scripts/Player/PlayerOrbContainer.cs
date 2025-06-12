using com.game.itemsystem;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerOrbContainer : MonoBehaviour
    {
        [SerializeField] private PlayerOrbController m_targetController;

        Dictionary<SimpleOrb, OrbInventory> m_orbInventoryEntries = new();
        List<OrbItemProfile> m_restoredUpgradeCache;
        List<OrbItemProfile> m_upgradeCache;
        Stack<OrbInventoryChange> m_undoCache = new();

        public Dictionary<SimpleOrb, OrbInventory> OrbInventoryEntries => m_orbInventoryEntries;
        public List<OrbItemProfile> RestoredUpgradeCache { get { return m_restoredUpgradeCache; } private set { m_restoredUpgradeCache = value; } }
        public List<OrbItemProfile> UpgradeCache { get { return m_upgradeCache; } set { m_upgradeCache = value; } }
        public Stack<OrbInventoryChange> UndoCache { get { return m_undoCache; } set { m_undoCache = value; } }

        public PlayerOrbController Controller => m_targetController;

        public bool Initialized { get; private set; } = false;

        public event Action<SimpleOrb, SimpleOrb> OnOrbSwapped;
        public event Action OnUndoCacheApplied;

        event Action<PlayerOrbContainer> m_onInitialized;

        private void Start()
        {
            m_targetController.OnInitialize((_) =>
            {
                Refresh();
                Initialized = true;
                m_onInitialized?.Invoke(this);
            });
        }

        private void Update()
        {
            foreach (var kvp in m_orbInventoryEntries)
            {
                kvp.Value.Update();
            }
        }

        public void SetUpgradeCache(IEnumerable<OrbItemProfile> enumerable)
        {
            if (enumerable == null)
            {
                m_upgradeCache = null;
                m_restoredUpgradeCache = null;
                return;
            }

            m_upgradeCache = new List<OrbItemProfile>(enumerable);
            m_restoredUpgradeCache = new List<OrbItemProfile>(enumerable);
        }

        public void Refresh()
        {
            Dictionary<SimpleOrb, OrbInventory> temp = new();
            foreach (SimpleOrb orb in m_targetController.orbsOnEllipse)
            {
                if (m_orbInventoryEntries.TryGetValue(orb, out OrbInventory inventory))
                    temp.Add(orb, inventory);
                else
                    temp.Add(orb, new OrbInventory(orb.Stats));
            }

            m_orbInventoryEntries = temp;
        }

        public void UndoAll()
        {
            m_upgradeCache = new(m_restoredUpgradeCache);

            while(m_undoCache.TryPop(out OrbInventoryChange lastChange))
            {
                lastChange.Undo();
            }

            ClearUndoHistory();
        }

        public void FetchRestoredUpgrades()
        {
            if (m_restoredUpgradeCache == null)
            {
                m_upgradeCache = null;
                return;
            }

            m_upgradeCache = new(m_restoredUpgradeCache);
        }

        public bool ApplyUpgrade(OrbItemProfile upgrade, SimpleOrb target)
        {
            if (!m_orbInventoryEntries.TryGetValue(target, out OrbInventory inventory))
                return false;

            OrbInventoryChange undo = new OrbInventoryAddItemChange(target, inventory, this);
            bool success = inventory.Add(new ItemObject<OrbItemProfile>(upgrade));

            if (success)
            {
                m_undoCache.Push(undo);

                if (m_upgradeCache != null && m_upgradeCache.Contains(upgrade)) 
                    m_upgradeCache.Remove(upgrade);
            }

            return success;
        }

        public void IrreversiblyApplyUndoCache()
        {
            while (m_undoCache.TryPop(out OrbInventoryChange lastChange))
            {
                lastChange.Dispose();
            }

            ClearUndoHistory();

            if (UpgradeCache != null)
                RestoredUpgradeCache = new(UpgradeCache);
            else
                RestoredUpgradeCache = null;

            FetchRestoredUpgrades();

            OnUndoCacheApplied?.Invoke();
        }

        public bool SwapOrb(SimpleOrb target, SimpleOrb prefab)
        {
            bool result = m_targetController.SwapOrb(target, prefab, out SimpleOrb newOrb);

            if (!result)
                return false;

            m_orbInventoryEntries.ChangeKey(target, newOrb);
            OnOrbSwapped?.Invoke(target, newOrb);

            return result;
        }

        public void ClearUndoHistory()
        {
            m_undoCache.Clear();
        }

        public void OnInitialize(Action<PlayerOrbContainer> callback)
        {
            if (!Initialized)
            {
                m_onInitialized += callback;
                return;
            }

            callback?.Invoke(this);
        }
    }
}
