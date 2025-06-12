using System.Collections.Generic;

namespace com.game.utilities
{
    public interface IOrbStickTarget 
    {
        public int StickedOrbCount { get; }
        public List<SimpleOrb> OrbsSticked { get; }

        void CommitOrbStick(SimpleOrb orb);
        void CommitOrbUnstick(SimpleOrb orb);
    }
}
