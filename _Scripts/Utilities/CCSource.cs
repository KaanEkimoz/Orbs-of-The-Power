using UnityEngine;

namespace com.game.utilities
{
    [System.Flags]
    public enum CCSource
    {
        None = 0,
        Unset = (1  << 0),
        Internal = (1 << 1),
        Ice = (1 << 2),
    }
}
