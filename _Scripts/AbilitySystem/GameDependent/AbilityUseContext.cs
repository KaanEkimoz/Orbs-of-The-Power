using com.game.orbsystem.statsystemextensions;
using com.game.player;

namespace com.game.abilitysystem.gamedependent
{
    public class AbilityUseContext
    {
        public SimpleOrb OrbSelected;
        public PlayerStats PlayerStats;
        public OrbStats OrbStats;
        public bool OrbWasThrown;
    }
}