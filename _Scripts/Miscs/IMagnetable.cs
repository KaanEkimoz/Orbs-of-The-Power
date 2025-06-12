using UnityEngine;

namespace com.game.miscs
{
    public interface IMagnetable
    {
        public bool IsMagnetable { get; }
        public float MagnetResistance { get; }
    }
}
