using com.game.itemsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;

namespace com.game.orbsystem
{
    public class OrbInventoryReplaceItemChange : OrbInventoryChange
    {
        ItemObject<OrbItemProfile> m_itemRemoved;

        public OrbInventoryReplaceItemChange(SimpleOrb orb, OrbInventory inventory, ItemObject<OrbItemProfile> itemRemoved, PlayerOrbContainer master) : base(orb, inventory, master)
        {
            m_itemRemoved = itemRemoved;
        }

        public override void Dispose()
        {
            // swap orb prefab.
        }

        public override void Undo()
        {
            m_inventory.RemoveCurrentElement();
            m_inventory.Add(m_itemRemoved);
        }
    }
}
