using com.game.player;
using System;

namespace com.game.orbsystem
{
    public abstract class OrbInventoryChange : IDisposable
    {
        protected PlayerOrbContainer m_master;
        protected SimpleOrb m_target;
        protected OrbInventory m_inventory;

        public OrbInventoryChange(SimpleOrb orb, OrbInventory inventory, PlayerOrbContainer master)
        {
            m_master = master;
            m_target = orb;
            m_inventory = inventory;
        }

        public abstract void Dispose();
        public abstract void Undo();
    }
}
