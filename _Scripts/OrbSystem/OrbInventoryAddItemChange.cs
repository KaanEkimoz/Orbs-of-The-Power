using com.game.itemsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using UnityEngine;

namespace com.game.orbsystem
{
    public class OrbInventoryAddItemChange : OrbInventoryChange
    {
        ItemObject<OrbItemProfile> m_previousItem;

        public OrbInventoryAddItemChange(SimpleOrb orb, OrbInventory inventory, PlayerOrbContainer master) : base(orb, inventory, master)
        {
            m_previousItem = inventory.CurrentItem;
        }

        public override void Dispose()
        {
            SimpleOrb prefab = m_inventory.CastedProfile.Prefab;

            if (prefab == null)
                return;

            bool result = m_master.SwapOrb(m_target, prefab);
            if (result) m_target.gameObject.SetActive(false);
        }

        public override void Undo()
        {
            m_inventory.RemoveCurrentElement();

            if (m_previousItem == null)
                return;

            m_inventory.Add(m_previousItem);
        }
    }
}
